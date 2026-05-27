using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TinyRPG.UI
{
    public class CastbarUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float fadeStartTime = .25f;
        [SerializeField] private float fadeTime = .5f;

        [SerializeField] private Color normalColor;
        [SerializeField] private Color canceledColor;

        private Image castbarFill;
        private TMP_Text castbarText;
        private CanvasGroup canvasGroup;

        private ICastable activeCastSource;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            castbarFill = transform.Find("CastbarFill").GetComponent<Image>();
            castbarText = transform.Find("CastbarText").GetComponent<TMP_Text>();
            canvasGroup = GetComponent<CanvasGroup>();

            Hide();
        }

        private void Update()
        {
            if (activeCastSource == null || !activeCastSource.IsCurrentlyCasting) return;

            UpdateProgressBar(activeCastSource.CastProgress, activeCastSource.InvertFill);
        }

        public void SetCastSource(ICastable source)
        {
            activeCastSource = source;
            if(activeCastSource != null && activeCastSource.IsCurrentlyCasting)
            {
                Show(source.CastActionName);
            }
        }

        public void Show(string prompt)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            castbarFill.color = normalColor;
            castbarText.text = prompt;
            castbarFill.fillAmount = 0;

            canvasGroup.alpha = 1;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Cancel()
        {
            castbarText.text = "Canceled";
            castbarFill.color = canceledColor;

            fadeCoroutine = StartCoroutine(CanceledCoroutine());
        }

        IEnumerator CanceledCoroutine()
        {
            float elapsed = 0f;

            float startValue = 1f;
            float endValue = 0f;

            yield return new WaitForSeconds(fadeStartTime);

            while (elapsed < fadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(startValue, endValue, elapsed / fadeTime);

                elapsed += Time.deltaTime;
                yield return null;
            }

            Hide();
        }

        public void UpdateProgressBar(float normalizedFillAmount, bool invertFill)
        {
            float fillAmount = invertFill ? normalizedFillAmount : 1 - normalizedFillAmount;

            castbarFill.fillAmount = fillAmount;
        }
    }

}