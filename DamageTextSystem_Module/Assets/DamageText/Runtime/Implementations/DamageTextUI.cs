using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Povet.DamageText.Implementations
{
    /// <summary>
    /// 독립 Canvas 기반 UI 데미지 텍스트
    /// 각 텍스트가 자체 Canvas를 가져 Rebuild 격리
    /// 권장 사용: 동시 표시 개수 100개 이하
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class DamageTextUI : Core.DamageTextBase
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Camera targetCamera;

        [Header("Canvas Settings")]
        [SerializeField] private float canvasWorldSize = 1f; // World Space Canvas 크기
        [SerializeField] private int sortingOrder = 100; // 렌더링 순서

        private Color originalColor;

        private void Awake()
        {
            InitializeComponents();
            SetupCanvas();
        }

        private void InitializeComponents()
        {
            if (canvas == null) canvas = GetComponent<Canvas>();
            if (canvasScaler == null) canvasScaler = GetComponent<CanvasScaler>();
            if (tmpText == null) tmpText = GetComponentInChildren<TMP_Text>();
            if (rectTransform == null && tmpText != null)
                rectTransform = tmpText.GetComponent<RectTransform>();
            if (targetCamera == null) targetCamera = Camera.main;
        }

        private void SetupCanvas()
        {
            if (canvas == null) return;

            // World Space Canvas 설정
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = targetCamera;
            canvas.sortingOrder = sortingOrder;

            // Canvas 크기 설정
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                canvasRect.sizeDelta = new Vector2(canvasWorldSize, canvasWorldSize);
                canvasRect.localScale = Vector3.one * 0.01f; // World Space 스케일 조정
            }

            // CanvasScaler 설정
            if (canvasScaler != null)
            {
                canvasScaler.dynamicPixelsPerUnit = 100;
            }
        }

        protected override void SetDamageText(string text)
        {
            if (tmpText == null) return;
            tmpText.text = text;
        }

        protected override void SetWorldPosition(Vector3 worldPosition)
        {
            if (canvas == null) return;

            // World Space Canvas는 Transform 직접 사용
            transform.position = worldPosition;
        }

        protected override void SetLocalScale(Vector3 scale)
        {
            if (rectTransform == null) return;
            rectTransform.localScale = scale;
        }

        protected override void SetAlpha(float alpha)
        {
            if (tmpText == null) return;

            Color color = tmpText.color;
            color.a = originalColor.a * alpha;
            tmpText.color = color;
        }

        protected override void ApplyVisualStyle(Data.DamageTextStyle style)
        {
            if (tmpText == null || style == null) return;

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

        private void OnValidate()
        {
            InitializeComponents();
        }
    }
}