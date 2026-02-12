using UnityEngine;
using TMPro;

namespace Povet.DamageText.Implementations
{
    /// <summary>
    /// World Space 3D 데미지 텍스트
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageText3D : Core.DamageTextBase
    {
        [SerializeField] private TextMeshPro tmpText;
        [SerializeField] private Transform textTransform;
        [SerializeField] private bool useBillboard = true;
        [SerializeField] private Camera targetCamera;

        private Color originalColor;

        private void Awake()
        {
            if (tmpText == null) tmpText = GetComponent<TextMeshPro>();
            if (textTransform == null) textTransform = transform;
            if (targetCamera == null) targetCamera = Camera.main;
        }

        protected override void SetDamageText(string text)
        {
            tmpText.text = text;
        }

        protected override void SetWorldPosition(Vector3 worldPosition)
        {
            textTransform.position = worldPosition;
        }

        protected override void SetLocalScale(Vector3 scale)
        {
            textTransform.localScale = scale;
        }

        protected override void SetAlpha(float alpha)
        {
            Color color = tmpText.color;
            color.a = originalColor.a * alpha;
            tmpText.color = color;
        }

        protected override void ApplyVisualStyle(Data.DamageTextStyle style)
        {
            // 색상
            tmpText.color = style.TextColor;
            originalColor = style.TextColor;

            // 폰트 크기
            tmpText.fontSize = style.FontSize;

            // 아웃라인
            if (style.UseOutline)
            {
                tmpText.outlineColor = style.OutlineColor;
                tmpText.outlineWidth = style.OutlineWidth;
            }
            else
            {
                tmpText.outlineWidth = 0f;
            }
        }

        protected override void Update()
        {
            base.Update();

            // Billboard: 항상 카메라를 향하도록
            if (useBillboard && targetCamera != null && IsActive)
            {
                textTransform.rotation = Quaternion.LookRotation(
                    textTransform.position - targetCamera.transform.position
                );
            }
        }

        // 에디터에서 자동 참조 설정
        private void OnValidate()
        {
            if (tmpText == null) tmpText = GetComponent<TextMeshPro>();
            if (textTransform == null) textTransform = transform;
        }
    }
}