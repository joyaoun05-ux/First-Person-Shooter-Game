using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [SerializeField] private int startingHealth = 6;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private int cashValue = 50;

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

        Debug.Log("Wave " + waveNumber + " enemy health = " + currentHealth);
    }

    private void Die()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

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