using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitMarkerUI : MonoBehaviour
{
    [SerializeField] private Image hitMarkerImage;

    [Header("Timing")]
    [SerializeField] private float displayTime = 0.08f;
    [SerializeField] private float fadeSpeed = 12f;

    [Header("Scale Animation")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float popScale = 1.3f;
    [SerializeField] private float scaleLerpSpeed = 12f;

    [Header("Colors")]
    [SerializeField] private Color normalHitColor = Color.white;
    [SerializeField] private Color killHitColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip killSound;

    private Coroutine hitRoutine;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = hitMarkerImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        SetAlpha(0f);
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * normalScale;
        }
    }

    public void ShowHitMarker(bool wasKill = false)
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }

        hitRoutine = StartCoroutine(HitMarkerRoutine(wasKill));
    }

    private IEnumerator HitMarkerRoutine(bool wasKill)
    {
        // Color
        hitMarkerImage.color = wasKill ? killHitColor : normalHitColor;
        SetAlpha(1f);

        // Sound
        if (audioSource != null)
        {
            if (wasKill && killSound != null)
            {
                audioSource.PlayOneShot(killSound);
            }
            else if (!wasKill && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }

        // Scale pop
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * popScale;
        }

        float timer = 0f;

        while (timer < displayTime)
        {
            timer += Time.deltaTime;

            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.Lerp(
                    rectTransform.localScale,
                    Vector3.one * normalScale,
                    scaleLerpSpeed * Time.deltaTime
                );
            }

            yield return null;
        }

        while (hitMarkerImage.color.a > 0.01f)
        {
            float newAlpha = Mathf.Lerp(hitMarkerImage.color.a, 0f, fadeSpeed * Time.deltaTime);
            SetAlpha(newAlpha);

            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.Lerp(
                    rectTransform.localScale,
                    Vector3.one * normalScale,
                    scaleLerpSpeed * Time.deltaTime
                );
            }

            yield return null;
        }

        SetAlpha(0f);

        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * normalScale;
        }

        hitRoutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color color = hitMarkerImage.color;
        color.a = alpha;
        hitMarkerImage.color = color;
    }
}