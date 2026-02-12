namespace Povet.DamageText.Pool
{
    public interface IDamageTextPool
    {
        /// <summary>
        /// Pool에서 데미지 텍스트 가져오기
        /// </summary>
        Core.IDamageText Get();

        /// <summary>
        /// Pool에 데미지 텍스트 반환
        /// </summary>
        void Return(Core.IDamageText damageText);

        /// <summary>
        /// 초기 Pool 생성 (Warmup)
        /// </summary>
        void Initialize(int initialSize);

        /// <summary>
        /// Pool 정리
        /// </summary>
        void Clear();

        /// <summary>
        /// 현재 활성화된 객체 수
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// 현재 Pool에 대기 중인 객체 수
        /// </summary>
        int InactiveCount { get; }
    }
}