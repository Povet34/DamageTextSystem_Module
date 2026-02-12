using UnityEngine;
using TMPro;

namespace Povet.DamageText.Implementations
{
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
            if (tmpText == null) return;
            tmpText.text = text;
        }

        protected override void SetWorldPosition(Vector3 worldPosition)
        {
            if (textTransform == null) return;
            textTransform.position = worldPosition;
        }

        protected override void SetLocalScale(Vector3 scale)
        {
            if (textTransform == null) return;
            textTransform.localScale = scale;
        }

        protected override void SetAlpha(float alpha)
        {
            //if (tmpText == null) return;
            //tmpText.alpha = alpha;
        }

        protected override void ApplyVisualStyle(Data.DamageTextStyle style)
        {
            if (tmpText == null || style == null) return;

            Renderer rend = tmpText.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.sortingOrder = style.SortingOrder;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (useBillboard && targetCamera != null && IsActive)
            {
                //textTransform.rotation = Quaternion.LookRotation(
                //    textTransform.position - targetCamera.transform.position
                //);
                textTransform.forward = targetCamera.transform.forward;
            }
        }

        private void OnValidate()
        {
            if (tmpText == null) tmpText = GetComponent<TextMeshPro>();
            if (textTransform == null) textTransform = transform;
        }
    }
}