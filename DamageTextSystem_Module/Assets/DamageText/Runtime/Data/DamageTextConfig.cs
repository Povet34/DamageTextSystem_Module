using UnityEngine;
using System.Collections.Generic;

namespace Povet.DamageText.Data
{
    [CreateAssetMenu(fileName = "DamageTextConfig", menuName = "Povet/DamageText/Config")]
    public class DamageTextConfig : ScriptableObject
    {
        [Header("Styles")]
        public List<DamageTextStyle> Styles = new List<DamageTextStyle>();
        public DamageTextStyle DefaultStyle;

        [Header("Pool Settings")]
        public int InitialPoolSize = 20;
        public int MaxPoolSize = 100;

        /// <summary>
        /// 이름으로 스타일 찾기
        /// </summary>
        public DamageTextStyle GetStyle(string styleName)
        {
            var style = Styles.Find(s => s.StyleName == styleName);
            return style != null ? style : DefaultStyle;
        }
    }
}