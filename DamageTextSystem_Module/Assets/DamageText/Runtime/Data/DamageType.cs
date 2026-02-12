namespace Povet.DamageText.Data
{
    //일단은 미사용. DamageTextStyle의 StyleName을 enum으로 대체하는 방안 검토 중.
    public enum DamageType
    {
        Normal,      // 일반 데미지
        Critical,    // 크리티컬
        Weakness,    // 약점
        Miss         // 미스
    }
}