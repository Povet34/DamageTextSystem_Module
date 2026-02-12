using Povet.DamageText.Data;

namespace Povet.DamageText.Core
{
    public interface IDamageText
    {
        /// <summary>
        /// 데미지 텍스트 초기화 및 표시
        /// </summary>
        void Show(DamageTextData data);

        /// <summary>
        /// 데미지 텍스트 숨김 및 풀 반환
        /// </summary>
        void Hide();

        /// <summary>
        /// 현재 활성화 상태
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// 애니메이션 완료 콜백
        /// </summary>
        System.Action<IDamageText> OnComplete { get; set; }
    }
}