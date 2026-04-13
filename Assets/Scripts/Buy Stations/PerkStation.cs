using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PerkStation : MonoBehaviour
{
    public enum PerkType
    {
        MaxHealth,
        Damage
    }

    [Header("Perk Settings")]
    [SerializeField] private PerkType perkType;
    [SerializeField] private int cost = 200;
    [SerializeField] private int priceIncreasePerUse = 50;
    [SerializeField] private int maxCost = 800;
    [SerializeField] private int perkAmount = 20;

    [Header("Unlock Settings")]
    [SerializeField] private int unlockWave = 5;

    [Header("UI / Audio")]
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private AudioSource perkSound;
    [SerializeField] private AudioSource errorAudioSource;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private float useCooldown = 1.5f;

    private bool playerInRange = false;
    private float lastUseTime = -999f;
    private EnemySpawner enemySpawner;

    private void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Time.time - lastUseTime >= useCooldown)
            {
                TryBuyPerk();
                lastUseTime = Time.time;
            }
        }
    }

    private void TryBuyPerk()
    {
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner not found.");
            return;
        }

        if (enemySpawner.currentWave < unlockWave)
        {
            if (interactText != null)
                interactText.text = perkType + " Perk unlocks at Wave " + unlockWave;

            CancelInvoke(nameof(ResetInteractText));
            Invoke(nameof(ResetInteractText), 1f);
            return;
        }

        if (CashManager.Instance == null)
        {
            Debug.LogError("CashManager missing.");
            return;
        }

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        PlayerBuffs playerBuffs = FindObjectOfType<PlayerBuffs>();

        if (!CashManager.Instance.SpendCash(cost))
        {
            if (interactText != null)
                interactText.text = "Not enough cash!";

            if (errorAudioSource != null && errorSound != null)
            {
                errorAudioSource.PlayOneShot(errorSound);
            }

            CancelInvoke(nameof(ResetInteractText));
            Invoke(nameof(ResetInteractText), 1f);
            return;
        }

        switch (perkType)
        {
            case PerkType.MaxHealth:
                if (playerHealth != null)
                {
                    playerHealth.IncreaseMaxHealth(perkAmount, true);
                }
                break;

            case PerkType.Damage:
                if (playerBuffs != null)
                {
                    playerBuffs.AddDamageBonus(perkAmount);
                }
                break;
        }

        if (perkSound != null)
            perkSound.Play();

        IncreaseCost();

        if (interactText != null)
            interactText.text = GetSuccessMessage();

        CancelInvoke(nameof(ResetInteractText));
        Invoke(nameof(ResetInteractText), 1f);
    }

    private void IncreaseCost()
    {
        cost += priceIncreasePerUse;

        if (cost > maxCost)
            cost = maxCost;
    }

    private string GetPromptText()
    {
        if (enemySpawner != null && enemySpawner.currentWave < unlockWave)
        {
            return perkType + " Perk locked until Wave " + unlockWave;
        }

        return "Press E to buy " + perkType + " Perk ($" + cost + ")";
    }

    private string GetSuccessMessage()
    {
        switch (perkType)
        {
            case PerkType.MaxHealth:
                return "Max health increased!";
            case PerkType.Damage:
                return "Damage increased!";
            default:
                return "Perk purchased!";
        }
    }

    private void ResetInteractText()
    {
        if (!playerInRange || interactText == null)
            return;

        interactText.text = GetPromptText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInRange = true;
            ResetInteractText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactText != null)
                interactText.text = "";
        }
    }
}