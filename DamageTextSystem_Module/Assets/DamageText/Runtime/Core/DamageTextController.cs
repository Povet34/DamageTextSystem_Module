using UnityEngine;

namespace Povet.DamageText.Core
{
    /// <summary>
    /// 데미지 텍스트 시스템 메인 컨트롤러
    /// Pool 관리 및 데미지 텍스트 생성 담당
    /// </summary>
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
                    instance = FindObjectOfType<DamageTextController>();

                    if (instance == null)
                    {
                        Debug.LogWarning("[DamageTextController] Scene에 인스턴스가 없음");
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            // Singleton 설정
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[DamageTextController] 중복 인스턴스 감지, 파괴함");
                Destroy(gameObject);
                return;
            }

            instance = this;

            ValidateReferences();
        }

        private void ValidateReferences()
        {
            if (config == null)
            {
                Debug.LogError("[DamageTextController] Config가 설정되지 않음");
            }

            if (pool == null)
            {
                Debug.LogError("[DamageTextController] Pool이 설정되지 않음");
            }
        }

        /// <summary>
        /// 데미지 텍스트 표시 (스타일 이름으로)
        /// </summary>
        public IDamageText Show(int damageValue, string styleName, Vector3 worldPosition)
        {
            if (config == null || pool == null)
            {
                Debug.LogError("[DamageTextController] 필수 참조가 설정되지 않음");
                return null;
            }

            // 스타일 가져오기
            Data.DamageTextStyle style = config.GetStyle(styleName);

            if (style == null)
            {
                Debug.LogWarning($"[DamageTextController] 스타일을 찾을 수 없음: {styleName}, DefaultStyle 사용");
                style = config.DefaultStyle;
            }

            return Show(damageValue, style, worldPosition);
        }

        /// <summary>
        /// 데미지 텍스트 표시 (스타일 직접 지정)
        /// </summary>
        public IDamageText Show(int damageValue, Data.DamageTextStyle style, Vector3 worldPosition)
        {
            if (pool == null)
            {
                Debug.LogError("[DamageTextController] Pool이 없음");
                return null;
            }

            if (style == null)
            {
                Debug.LogError("[DamageTextController] Style이 null");
                return null;
            }

            // Pool에서 가져오기
            IDamageText damageText = pool.Get();

            if (damageText == null)
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning("[DamageTextController] Pool에서 객체를 가져올 수 없음 (최대 개수 도달)");
                }
                return null;
            }

            // 데이터 생성 및 표시
            Data.DamageTextData data = new Data.DamageTextData(damageValue, style, worldPosition);
            damageText.Show(data);

            if (showDebugInfo)
            {
                Debug.Log($"[DamageTextController] 데미지 텍스트 표시: {damageValue}, Style: {style.StyleName}, Pos: {worldPosition}");
            }

            return damageText;
        }

        /// <summary>
        /// 데미지 텍스트 표시 (DamageTextData로)
        /// </summary>
        public IDamageText Show(Data.DamageTextData data)
        {
            if (pool == null)
            {
                Debug.LogError("[DamageTextController] Pool이 없음");
                return null;
            }

            IDamageText damageText = pool.Get();

            if (damageText == null) return null;

            damageText.Show(data);
            return damageText;
        }

        /// <summary>
        /// 모든 활성 데미지 텍스트 강제 제거
        /// </summary>
        public void ClearAll()
        {
            if (pool != null)
            {
                pool.Clear();
                pool.Initialize(config != null ? config.InitialPoolSize : 20);
            }
        }

        /// <summary>
        /// Pool 상태 정보 가져오기
        /// </summary>
        public (int active, int inactive) GetPoolStatus()
        {
            if (pool == null) return (0, 0);
            return (pool.ActiveCount, pool.InactiveCount);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        // 디버그용
        private void OnGUI()
        {
            if (!showDebugInfo || !Application.isPlaying) return;

            var (active, inactive) = GetPoolStatus();

            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Box($"[DamageText Controller]\nActive: {active}\nInactive: {inactive}");
            GUILayout.EndArea();
        }
    }
}