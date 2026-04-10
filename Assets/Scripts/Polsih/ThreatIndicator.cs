using UnityEngine;
using UnityEngine.UI;

public class ThreatIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Image threatImage;

    [Header("Settings")]
    [SerializeField] private float dangerDistance = 10f;
    [SerializeField] private float fadeSpeed = 4f;
    [SerializeField] private float maxAlpha = 0.8f;

    [Header("Flash Settings")]
    [SerializeField] private bool useFlashing = true;
    [SerializeField] private float flashSpeed = 6f;
    [SerializeField] private float minFlashAlpha = 0.2f;

    private void Start()
    {
        if (threatImage != null)
        {
            Color color = threatImage.color;
            color.a = 0f;
            threatImage.color = color;
            threatImage.enabled = false;
        }
    }

    private void Update()
    {
        if (player == null || threatImage == null) return;

        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>(true);

        float nearestDistance = Mathf.Infinity;

        foreach (EnemyHealth enemy in enemies)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            float distance = Vector3.Distance(player.position, enemy.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
            }
        }

        float targetAlpha = 0f;
        bool shouldShow = nearestDistance <= dangerDistance;

        if (shouldShow)
        {
            float t = 1f - (nearestDistance / dangerDistance);
            float baseAlpha = Mathf.Lerp(0f, maxAlpha, t);

            if (useFlashing)
            {
                float flash = Mathf.PingPong(Time.time * flashSpeed, 1f);
                targetAlpha = Mathf.Lerp(minFlashAlpha, baseAlpha, flash);
            }
            else
            {
                targetAlpha = baseAlpha;
            }
        }

        if (shouldShow)
        {
            if (!threatImage.enabled)
                threatImage.enabled = true;
        }

        Color currentColor = threatImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        threatImage.color = currentColor;

        if (!shouldShow && currentColor.a <= 0.01f)
        {
            threatImage.enabled = false;
        }
    }
}