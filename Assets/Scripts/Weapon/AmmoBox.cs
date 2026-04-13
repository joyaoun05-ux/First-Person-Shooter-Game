using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class AmmoBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int cost = 100;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private AudioSource pickupSound;
    [SerializeField] private AudioSource errorAudioSource;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private float rotateSpeed = 60f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.2f;
    [SerializeField] private float useCooldown = 1.5f;

    private bool playerInRange = false;
    private Vector3 startPos;
    private float lastUseTime = -999f;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (Time.time - lastUseTime >= useCooldown)
            {
                TryBuyAmmo();
                lastUseTime = Time.time;
            }
        }
    }

    private void TryBuyAmmo()
    {
        if (CashManager.Instance == null)
        {
            Debug.LogError("CashManager is missing.");
            return;
        }

        if (!CashManager.Instance.SpendCash(cost))
        {
            if (interactText != null)
                interactText.text = "Not enough cash!";

            if (errorAudioSource != null && errorSound != null)
            {
                errorAudioSource.PlayOneShot(errorSound);
            }

            return;
        }

        Weapon[] weapons = FindObjectsOfType<Weapon>(true);

        foreach (Weapon weapon in weapons)
        {
            weapon.RefillAmmo();
        }

        if (interactText != null)
            interactText.text = "Ammo refilled!";

        if (pickupSound != null)
        {
            pickupSound.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactText != null)
                interactText.text = "Press E to buy ammo ($" + cost + ")";
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