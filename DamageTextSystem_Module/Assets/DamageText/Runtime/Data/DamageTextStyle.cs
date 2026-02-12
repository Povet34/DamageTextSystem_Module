using UnityEngine;

namespace Povet.DamageText.Data
{
    [CreateAssetMenu(fileName = "DamageTextStyle", menuName = "Povet/DamageText/Style")]
    public class DamageTextStyle : ScriptableObject
    {
        [Header("Identification")]
        public string StyleName;

        [Header("Rendering")]
        public int SortingOrder = 0;

        [Header("Prefab")]
        public GameObject DamageTextPrefab; // ¡ç ÇÙ½É º¯°æ

        [Header("Visual (for runtime override if needed)")]
        public float FontSize = 36f;

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