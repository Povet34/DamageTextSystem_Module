using UnityEngine;
using System.Collections.Generic;

namespace Povet.DamageText.Pool
{
    public class DamageTextPool : MonoBehaviour, IDamageTextPool
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 20;
        [SerializeField] private int maxSize = 100;
        [SerializeField] private Transform poolParent;

        private Queue<Core.IDamageText> pool = new Queue<Core.IDamageText>();
        private HashSet<Core.IDamageText> activeObjects = new HashSet<Core.IDamageText>();

        public int ActiveCount => activeObjects.Count;
        public int InactiveCount => pool.Count;

        private void Awake()
        {
            if (poolParent == null)
            {
                GameObject parent = new GameObject($"{gameObject.name}_PoolParent");
                parent.transform.SetParent(transform);
                poolParent = parent.transform;
            }

            Initialize(initialSize);
        }

        public void Initialize(int size)
        {
            if (prefab == null)
            {
                Debug.LogError("[DamageTextPool] Prefab이 설정되지 않음");
                return;
            }

            for (int i = 0; i < size; i++)
            {
                CreateNewInstance();
            }
        }

        public Core.IDamageText Get()
        {
            Core.IDamageText damageText;

            // Pool에서 가져오기
            if (pool.Count > 0)
            {
                damageText = pool.Dequeue();
            }
            // Pool이 비었으면 새로 생성 (최대 개수 체크)
            else
            {
                if (activeObjects.Count >= maxSize)
                {
                    Debug.LogWarning($"[DamageTextPool] 최대 개수 도달: {maxSize}");
                    return null;
                }

                damageText = CreateNewInstance();
            }

            activeObjects.Add(damageText);
            return damageText;
        }

        public void Return(Core.IDamageText damageText)
        {
            if (damageText == null) return;

            if (!activeObjects.Remove(damageText))
            {
                Debug.LogWarning("[DamageTextPool] 이미 반환된 객체이거나 이 Pool의 객체가 아님");
                return;
            }

            // Pool에 반환
            pool.Enqueue(damageText);

            // 부모 설정
            if (damageText is MonoBehaviour mb)
            {
                mb.transform.SetParent(poolParent);
            }
        }

        public void Clear()
        {
            // 활성 객체 모두 제거
            foreach (var obj in activeObjects)
            {
                if (obj is MonoBehaviour mb)
                {
                    Destroy(mb.gameObject);
                }
            }
            activeObjects.Clear();

            // Pool의 객체 모두 제거
            while (pool.Count > 0)
            {
                var obj = pool.Dequeue();
                if (obj is MonoBehaviour mb)
                {
                    Destroy(mb.gameObject);
                }
            }
            pool.Clear();
        }

        private Core.IDamageText CreateNewInstance()
        {
            GameObject instance = Instantiate(prefab, poolParent);
            instance.SetActive(false);

            Core.IDamageText damageText = instance.GetComponent<Core.IDamageText>();

            if (damageText == null)
            {
                Debug.LogError($"[DamageTextPool] Prefab에 IDamageText 구현체가 없음: {prefab.name}");
                Destroy(instance);
                return null;
            }

            // 완료 콜백 설정
            damageText.OnComplete = Return;

            pool.Enqueue(damageText);
            return damageText;
        }

        private void OnDestroy()
        {
            Clear();
        }

        // 디버그용
        private void OnGUI()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                GUILayout.Label($"Pool: Active={ActiveCount}, Inactive={InactiveCount}");
            }
#endif
        }
    }
}