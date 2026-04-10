using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlashUI : MonoBehaviour
{
    [SerializeField] private Image damageImage;
    [SerializeField] private float flashAlpha = 0.4f;
    [SerializeField] private float fadeSpeed = 3f;

    private Coroutine flashCoroutine;

    private void Start()
    {
        if (damageImage != null)
        {
            Color color = damageImage.color;
            color.a = 0f;
            damageImage.color = color;
        }
    }

    public void ShowDamageFlash()
    {
        if (damageImage == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Color color = damageImage.color;
        color.a = flashAlpha;
        damageImage.color = color;

        while (damageImage.color.a > 0.01f)
        {
            color = damageImage.color;
            color.a = Mathf.Lerp(color.a, 0f, fadeSpeed * Time.deltaTime);
            damageImage.color = color;
            yield return null;
        }

        color.a = 0f;
        damageImage.color = color;
        flashCoroutine = null;
    }
}