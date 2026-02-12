using UnityEngine;

namespace Povet.DamageText.Core
{
    public class DamageTextController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Data.DamageTextConfig config;
        [SerializeField] private Pool.DamageTextPool pool;

        [Header("Runtime Info")]
        [SerializeField] private bool showDebugInfo;

        private static DamageTextController instance;
        public static DamageTextController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<DamageTextController>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            if (config == null || pool == null)
            {
                Debug.LogError("[DamageTextController] Config 또는 Pool이 없음");
                return;
            }

            // 각 Style별로 Pool 초기화
            foreach (var style in config.Styles)
            {
                if (style.DamageTextPrefab != null)
                {
                    pool.InitializeStylePool(style.StyleName, style.DamageTextPrefab, config.InitialPoolSize);
                }
                else
                {
                    Debug.LogWarning($"[DamageTextController] Prefab이 없음: {style.StyleName}");
                }
            }
        }

        public IDamageText Show(int damageValue, string styleName, Vector3 worldPosition)
        {
            if (config == null || pool == null) return null;

            Data.DamageTextStyle style = config.GetStyle(styleName);
            if (style == null)
            {
                Debug.LogWarning($"[DamageTextController] 스타일을 찾을 수 없음: {styleName}");
                return null;
            }

            // Style별 Pool에서 가져오기
            IDamageText damageText = pool.Get(styleName);
            if (damageText == null) return null;

            Data.DamageTextData data = new Data.DamageTextData(damageValue, style, worldPosition);
            damageText.Show(data);

            return damageText;
        }

        public void ClearAll()
        {
            if (pool != null)
            {
                pool.Clear();

                // 재초기화
                foreach (var style in config.Styles)
                {
                    if (style.DamageTextPrefab != null)
                    {
                        pool.InitializeStylePool(style.StyleName, style.DamageTextPrefab, config.InitialPoolSize);
                    }
                }
            }
        }

        public (int active, int inactive) GetPoolStatus()
        {
            if (pool == null) return (0, 0);
            return (pool.ActiveCount, pool.InactiveCount);
        }
    }
}