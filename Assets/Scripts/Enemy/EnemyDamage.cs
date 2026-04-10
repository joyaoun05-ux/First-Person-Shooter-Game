using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private int currentDamage;
    private float lastAttackTime;

    private void Awake()
    {
        currentDamage = baseDamage;
    }

    public void SetWaveDamage(int waveNumber)
    {
        currentDamage = baseDamage + (waveNumber - 1) * 2;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.CompareTag("Player")) return;

        if (Time.time - lastAttackTime < attackCooldown) return;

        PlayerHealth playerHealth = other.transform.root.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(currentDamage);
            lastAttackTime = Time.time;
            Debug.Log("Enemy dealt " + currentDamage + " damage to player");
        }
    }
}