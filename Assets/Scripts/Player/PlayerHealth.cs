using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private DamageFlashUI damageFlashUI;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private float hurtSoundCooldown = 0.5f;

    private int currentHealth;
    private bool isDead = false;
    private float lastHurtSoundTime = -999f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();

        if (damageFlashUI != null)
        {
            damageFlashUI.ShowDamageFlash();
        }

        if (audioSource != null && hurtClip != null)
        {
            if (Time.time - lastHurtSoundTime >= hurtSoundCooldown)
            {
                audioSource.PlayOneShot(hurtClip);
                lastHurtSoundTime = Time.time;
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player died");

        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
            StartCoroutine(DieRoutine());
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private System.Collections.IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }
}