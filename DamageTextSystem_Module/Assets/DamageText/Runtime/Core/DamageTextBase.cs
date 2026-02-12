using Povet.DamageText.Data;
using UnityEngine;

namespace Povet.DamageText.Core
{
    public abstract class DamageTextBase : MonoBehaviour, IDamageText
    {
        protected DamageTextData currentData;
        protected bool isPlaying;

        private float currentTime;
        private AnimationState animState;

        private Vector3 startPosition;
        private Vector3 startScale;

        public bool IsActive => isPlaying;
        public System.Action<IDamageText> OnComplete { get; set; }

        private enum AnimationState
        {
            Enter,
            Exit
        }

        public virtual void Show(DamageTextData data)
        {
            currentData = data;
            currentTime = 0f;
            isPlaying = true;
            animState = data.Style.HasEnterAnimation ? AnimationState.Enter : AnimationState.Exit;

            gameObject.SetActive(true);

            // 시작 위치/스케일 저장
            startPosition = data.WorldPosition;
            startScale = Vector3.one;

            // 위치 설정
            SetWorldPosition(startPosition);

            // 텍스트 설정
            SetDamageText(data.DamageValue.ToString());

            // 스타일 적용 (색상, 크기, 아웃라인 등)
            ApplyVisualStyle(data.Style);

            // 초기 상태 설정
            if (data.Style.HasEnterAnimation)
            {
                SetLocalScale(Vector3.zero);
                SetAlpha(1f);
            }
            else
            {
                SetLocalScale(startScale);
                SetAlpha(1f);
            }
        }

        public virtual void Hide()
        {
            isPlaying = false;
            gameObject.SetActive(false);
            OnComplete?.Invoke(this);
        }

        protected virtual void Update()
        {
            if (!isPlaying) return;

            currentTime += Time.deltaTime;

            switch (animState)
            {
                case AnimationState.Enter:
                    UpdateEnterAnimation();
                    break;

                case AnimationState.Exit:
                    UpdateExitAnimation();
                    break;
            }
        }

        private void UpdateEnterAnimation()
        {
            var style = currentData.Style;
            float normalizedTime = Mathf.Clamp01(currentTime / style.EnterDuration);

            // 스케일 애니메이션
            float scaleValue = style.EnterScaleCurve.Evaluate(normalizedTime);
            SetLocalScale(startScale * scaleValue);

            // 등장 애니메이션 완료
            if (currentTime >= style.EnterDuration)
            {
                animState = AnimationState.Exit;
                currentTime = 0f;
            }
        }

        private void UpdateExitAnimation()
        {
            var style = currentData.Style;
            float normalizedTime = Mathf.Clamp01(currentTime / style.ExitDuration);

            // 위로 이동
            Vector3 newPos = startPosition + Vector3.up * (style.MoveUpSpeed * currentTime);
            SetWorldPosition(newPos);

            // 스케일 감소
            float scaleValue = style.ScaleCurve.Evaluate(normalizedTime);
            SetLocalScale(startScale * scaleValue);

            // 알파 감소
            float alphaValue = style.AlphaCurve.Evaluate(normalizedTime);
            SetAlpha(alphaValue);

            // 애니메이션 완료
            if (currentTime >= style.ExitDuration)
            {
                Hide();
            }
        }

        // ===== 구현체에서 구현해야 할 추상 메서드 =====

        /// <summary>
        /// 데미지 값 텍스트 설정
        /// </summary>
        protected abstract void SetDamageText(string text);

        /// <summary>
        /// 월드 좌표 설정
        /// </summary>
        protected abstract void SetWorldPosition(Vector3 worldPosition);

        /// <summary>
        /// 로컬 스케일 설정
        /// </summary>
        protected abstract void SetLocalScale(Vector3 scale);

        /// <summary>
        /// 알파값 설정
        /// </summary>
        protected abstract void SetAlpha(float alpha);

        /// <summary>
        /// 비주얼 스타일 적용 (색상, 폰트 크기, 아웃라인 등)
        /// </summary>
        protected abstract void ApplyVisualStyle(Data.DamageTextStyle style);
    }
}