using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class MysteryBox : MonoBehaviour
{
    [Header("Box Settings")]
    [SerializeField] private int cost = 250;
    [SerializeField] private int priceIncreasePerUse = 50;
    [SerializeField] private int maxCost = 1000;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private float openDelay = 1.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource boxSound;
    [SerializeField] private AudioSource errorAudioSource;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private float boxSoundDuration = 1.0f; // how long box sound plays

    [Header("Weapon Rewards")]
    [SerializeField] private WeaponSwitcher weaponSwitcher;
    [SerializeField] private WeaponData[] possibleWeapons;

    [Tooltip("Same size as Possible Weapons. Higher weight = more likely.")]
    [SerializeField] private float[] rewardWeights;

    [Header("Fairness Settings")]
    [SerializeField] private bool avoidImmediateDuplicate = true;
    [SerializeField] private int duplicateRerollAttempts = 5;

    private bool playerInRange = false;
    private bool isRolling = false;
    private int lastRewardIndex = -1;

    private void Update()
    {
        UpdateInteractText();

        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!isRolling)
            {
                StartCoroutine(TryBuyMysteryBox());
            }
        }
    }

    private void UpdateInteractText()
    {
        if (interactText == null) return;

        if (playerInRange && !isRolling)
        {
            interactText.text = "Press E to buy Mystery Box ($" + cost + ")";
        }
        else if (!playerInRange)
        {
            interactText.text = "";
        }
    }

    private IEnumerator TryBuyMysteryBox()
    {
        if (CashManager.Instance == null)
        {
            Debug.LogError("CashManager.Instance is NULL");
            yield break;
        }

        if (weaponSwitcher == null)
        {
            Debug.LogError("WeaponSwitcher is NOT assigned");
            yield break;
        }

        if (possibleWeapons == null || possibleWeapons.Length == 0)
        {
            Debug.LogError("PossibleWeapons array is EMPTY");
            yield break;
        }

        if (!CashManager.Instance.SpendCash(cost))
        {
            if (interactText != null)
                interactText.text = "Not enough cash!";

            if (errorAudioSource != null && errorSound != null)
            {
                errorAudioSource.PlayOneShot(errorSound);
            }

            yield return new WaitForSeconds(1f);
            yield break;
        }

        isRolling = true;

        //  PLAY BOX SOUND (SHORTENED)
        if (boxSound != null)
        {
            boxSound.Play();
            StartCoroutine(StopBoxSoundAfterTime(boxSoundDuration));
        }

        if (interactText != null)
            interactText.text = "Rolling mystery box...";

        yield return new WaitForSeconds(openDelay);

        int selectedIndex = GetWeightedRandomWeaponIndex();
        WeaponData selectedWeaponData = possibleWeapons[selectedIndex];

        if (selectedWeaponData == null)
        {
            Debug.LogError("Selected WeaponData is NULL");
            isRolling = false;
            yield break;
        }

        weaponSwitcher.SwitchToWeaponData(selectedWeaponData);
        lastRewardIndex = selectedIndex;

        IncreaseCost();

        if (interactText != null)
            interactText.text = "You got: " + selectedWeaponData.name;

        yield return new WaitForSeconds(1.5f);

        isRolling = false;
        UpdateInteractText();
    }

    //  STOP SOUND AFTER SHORT TIME
    private IEnumerator StopBoxSoundAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (boxSound != null && boxSound.isPlaying)
        {
            boxSound.Stop();
        }
    }

    private void IncreaseCost()
    {
        cost += priceIncreasePerUse;

        if (cost > maxCost)
            cost = maxCost;
    }

    private int GetWeightedRandomWeaponIndex()
    {
        if (possibleWeapons.Length == 1)
            return 0;

        int chosenIndex = WeightedPick();

        if (!avoidImmediateDuplicate)
            return chosenIndex;

        int attempts = 0;

        while (possibleWeapons.Length > 1 && chosenIndex == lastRewardIndex && attempts < duplicateRerollAttempts)
        {
            chosenIndex = WeightedPick();
            attempts++;
        }

        return chosenIndex;
    }

    private int WeightedPick()
    {
        if (rewardWeights == null || rewardWeights.Length != possibleWeapons.Length)
        {
            return Random.Range(0, possibleWeapons.Length);
        }

        float totalWeight = 0f;

        for (int i = 0; i < rewardWeights.Length; i++)
        {
            totalWeight += Mathf.Max(0f, rewardWeights[i]);
        }

        if (totalWeight <= 0f)
        {
            return Random.Range(0, possibleWeapons.Length);
        }

        float randomValue = Random.Range(0f, totalWeight);
        float runningTotal = 0f;

        for (int i = 0; i < rewardWeights.Length; i++)
        {
            runningTotal += Mathf.Max(0f, rewardWeights[i]);

            if (randomValue <= runningTotal)
                return i;
        }

        return possibleWeapons.Length - 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInRange = false;
            UpdateInteractText();
        }
    }
}