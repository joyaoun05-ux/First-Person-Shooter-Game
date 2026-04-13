using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    public enum EnemyType
    {
        Normal,
        Healer,
        Tank,
        Shield
    }

    [Header("Type")]
    [SerializeField] private EnemyType enemyType = EnemyType.Normal;
    public EnemyType Type => enemyType;

    [Header("Stats")]
    [SerializeField] private int startingHealth = 6;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private int cashValue = 50;

    [Header("Shield Settings")]
    [SerializeField] private bool hasShield = false;
    [SerializeField] private float frontDamageMultiplier = 0.3f; // 30% damage from front

    [Header("Death VFX")]
    [SerializeField] private GameObject deathVFXPrefab;

    [Header("Death Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathVolume = 1f;

    [Header("Heal On Death")]
    [SerializeField] private bool healPlayerOnDeath = false;
    [SerializeField] private int healAmountOnDeath = 20;

    private int currentHealth;
    private int bonusHealth = 0;

    public event Action<EnemyHealth> OnDied;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public bool TakeDamage(int damage, Vector3 hitDirection)
    {
        if (hasShield)
        {
            float dot = Vector3.Dot(transform.forward, -hitDirection.normalized);

            // if hit from front
            if (dot > 0.5f)
            {
                damage = Mathf.RoundToInt(damage * frontDamageMultiplier);
            }
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }

        return false;
    }

    public void SetWaveDifficulty(int waveNumber)
    {
        bonusHealth = (waveNumber - 1) * 2;
        currentHealth = startingHealth + bonusHealth;
    }

    private void Die()
    {
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, deathVolume);
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        if (CashManager.Instance != null)
        {
            CashManager.Instance.AddCash(cashValue);
        }

        if (healPlayerOnDeath)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmountOnDeath);
            }
        }

        OnDied?.Invoke(this);
    }

    public void OnGetFromPool()
    {
        currentHealth = startingHealth + bonusHealth;
    }

    public void OnReturnFromPool()
    {
        OnDied = null;
    }
}