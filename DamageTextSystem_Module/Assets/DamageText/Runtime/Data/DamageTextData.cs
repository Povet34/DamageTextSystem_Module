using UnityEngine;

namespace Povet.DamageText.Data
{
    public struct DamageTextData
    {
        public int DamageValue;
        public DamageTextStyle Style;
        public Vector3 WorldPosition;

        public DamageTextData(int damageValue, DamageTextStyle style, Vector3 worldPosition)
        {
            DamageValue = damageValue;
            Style = style;
            WorldPosition = worldPosition;
        }
    }
}