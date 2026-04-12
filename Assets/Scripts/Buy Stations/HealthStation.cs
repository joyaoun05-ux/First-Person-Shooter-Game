using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HealthStation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int cost = 150;
    [SerializeField] private int priceIncreasePerUse = 25;
    [SerializeField] private int maxCost = 500;
    [SerializeField] private int healAmount = 40;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private AudioSource healSound;
    [SerializeField] private float useCooldown = 1.5f;

    private bool playerInRange = false;
    private float lastUseTime = -999f;

    private void Update()
    {
        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Time.time - lastUseTime >= useCooldown)
            {
                TryBuyHeal();
                lastUseTime = Time.time;
            }
        }
    }

    private void TryBuyHeal()
    {
        if (CashManager.Instance == null)
        {
            Debug.LogError("CashManager missing.");
            return;
        }

        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth missing.");
            return;
        }

        if (!CashManager.Instance.SpendCash(cost))
        {
            if (interactText != null)
                interactText.text = "Not enough cash!";
            CancelInvoke(nameof(ResetInteractText));
            Invoke(nameof(ResetInteractText), 1f);
            return;
        }

        playerHealth.Heal(healAmount);

        IncreaseCost();

        if (interactText != null)
            interactText.text = "Health restored!";

        if (healSound != null)
            healSound.Play();

        CancelInvoke(nameof(ResetInteractText));
        Invoke(nameof(ResetInteractText), 1f);
    }

    private void IncreaseCost()
    {
        cost += priceIncreasePerUse;

        if (cost > maxCost)
            cost = maxCost;

        Debug.Log("Health station new cost: " + cost);
    }

    private void ResetInteractText()
    {
        if (playerInRange && interactText != null)
        {
            interactText.text = "Press E to heal ($" + cost + ")";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactText != null)
                interactText.text = "Press E to heal ($" + cost + ")";
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