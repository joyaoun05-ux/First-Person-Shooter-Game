using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [Header("Stats")]
    [SerializeField] private int startingHealth = 6;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private int cashValue = 50;

    [Header("Death VFX")]
    [SerializeField] private GameObject deathVFXPrefab;

    private int currentHealth;
    private int bonusHealth = 0;

    public event Action<EnemyHealth> OnDied;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public bool TakeDamage(int damage)
    {
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
        // Spawn death VFX
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        }

        // Score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // Cash
        if (CashManager.Instance != null)
        {
            CashManager.Instance.AddCash(cashValue);
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