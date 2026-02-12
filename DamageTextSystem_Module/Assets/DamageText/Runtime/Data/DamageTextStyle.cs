using UnityEngine;

namespace Povet.DamageText.Data
{
    [CreateAssetMenu(fileName = "DamageTextStyle", menuName = "Povet/DamageText/Style")]
    public class DamageTextStyle : ScriptableObject
    {
        [Header("Identification")]
        public string StyleName;

        [Header("Visual")]
        public Color TextColor = Color.white;
        public float FontSize = 36f;
        public bool UseOutline;
        public Color OutlineColor = Color.black;
        public float OutlineWidth = 0.2f;

        [Header("Animation")]
        public bool HasEnterAnimation;
        public AnimationCurve EnterScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float EnterDuration = 0.2f;

        [Header("Exit Animation")]
        public float ExitDuration = 1.0f;
        public float MoveUpSpeed = 1.0f;
        public AnimationCurve ScaleCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);
        public AnimationCurve AlphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    }
}