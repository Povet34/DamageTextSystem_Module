using UnityEngine;
using System.Collections.Generic;

namespace Povet.DamageText.Pool
{
    public class DamageTextPool : MonoBehaviour, IDamageTextPool
    {
        [Header("Pool Settings")]
        [SerializeField] private int initialSize = 20;
        [SerializeField] private int maxSize = 100;
        [SerializeField] private Transform poolParent;

        // Style별 Pool 관리
        private Dictionary<string, Queue<Core.IDamageText>> pools = new Dictionary<string, Queue<Core.IDamageText>>();
        private Dictionary<string, HashSet<Core.IDamageText>> activePools = new Dictionary<string, HashSet<Core.IDamageText>>();
        private Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();

        public int ActiveCount
        {
            get
            {
                int total = 0;
                foreach (var pool in activePools.Values)
                {
                    total += pool.Count;
                }
                return total;
            }
        }

        public int InactiveCount
        {
            get
            {
                int total = 0;
                foreach (var pool in pools.Values)
                {
                    total += pool.Count;
                }
                return total;
            }
        }

        private void Awake()
        {
            if (poolParent == null)
            {
                GameObject parent = new GameObject($"{gameObject.name}_PoolParent");
                parent.transform.SetParent(transform);
                poolParent = parent.transform;
            }
        }

        public void Initialize(int size)
        {
            // 초기화는 Controller에서 Style별로 진행
        }

        /// <summary>
        /// 특정 스타일의 Prefab으로 Pool 초기화
        /// </summary>
        public void InitializeStylePool(string styleName, GameObject prefab, int size)
        {
            if (prefab == null)
            {
                Debug.LogError($"[DamageTextPool] Prefab이 null: {styleName}");
                return;
            }

            if (!pools.ContainsKey(styleName))
            {
                pools[styleName] = new Queue<Core.IDamageText>();
                activePools[styleName] = new HashSet<Core.IDamageText>();
                prefabCache[styleName] = prefab;
            }

            for (int i = 0; i < size; i++)
            {
                CreateNewInstance(styleName, prefab);
            }
        }

        /// <summary>
        /// 특정 스타일의 데미지 텍스트 가져오기
        /// </summary>
        public Core.IDamageText Get(string styleName)
        {
            if (!pools.ContainsKey(styleName))
            {
                Debug.LogError($"[DamageTextPool] 초기화되지 않은 스타일: {styleName}");
                return null;
            }

            Core.IDamageText damageText;

            // Pool에서 가져오기
            if (pools[styleName].Count > 0)
            {
                damageText = pools[styleName].Dequeue();
            }
            // Pool이 비었으면 새로 생성
            else
            {
                int totalActive = ActiveCount;
                if (totalActive >= maxSize)
                {
                    Debug.LogWarning($"[DamageTextPool] 최대 개수 도달: {maxSize}");
                    return null;
                }

                damageText = CreateNewInstance(styleName, prefabCache[styleName]);
            }

            activePools[styleName].Add(damageText);
            return damageText;
        }

        public Core.IDamageText Get()
        {
            Debug.LogError("[DamageTextPool] Style 없이 Get() 호출됨. Get(styleName) 사용 필요");
            return null;
        }

        public void Return(Core.IDamageText damageText)
        {
            if (damageText == null) return;

            // 어느 Pool에 속하는지 찾기
            string styleName = null;
            foreach (var kvp in activePools)
            {
                if (kvp.Value.Contains(damageText))
                {
                    styleName = kvp.Key;
                    break;
                }
            }

            if (styleName == null)
            {
                Debug.LogWarning("[DamageTextPool] 이미 반환된 객체이거나 이 Pool의 객체가 아님");
                return;
            }

            activePools[styleName].Remove(damageText);
            pools[styleName].Enqueue(damageText);

            // 부모 설정
            if (damageText is MonoBehaviour mb)
            {
                mb.transform.SetParent(poolParent);
            }
        }

        public void Clear()
        {
            // 모든 Pool 정리
            foreach (var pool in activePools.Values)
            {
                foreach (var obj in pool)
                {
                    if (obj is MonoBehaviour mb)
                    {
                        Destroy(mb.gameObject);
                    }
                }
            }

            foreach (var pool in pools.Values)
            {
                while (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    if (obj is MonoBehaviour mb)
                    {
                        Destroy(mb.gameObject);
                    }
                }
            }

            pools.Clear();
            activePools.Clear();
            prefabCache.Clear();
        }

        private Core.IDamageText CreateNewInstance(string styleName, GameObject prefab)
        {
            GameObject instance = Instantiate(prefab, poolParent);
            instance.SetActive(false);
            instance.name = $"{styleName}_{pools[styleName].Count}";

            Core.IDamageText damageText = instance.GetComponent<Core.IDamageText>();

            if (damageText == null)
            {
                Debug.LogError($"[DamageTextPool] Prefab에 IDamageText 구현체가 없음: {prefab.name}");
                Destroy(instance);
                return null;
            }

            // 완료 콜백 설정
            damageText.OnComplete = Return;

            pools[styleName].Enqueue(damageText);
            return damageText;
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}