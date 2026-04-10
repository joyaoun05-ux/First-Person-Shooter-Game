using UnityEngine;
using TMPro;

public class CashManager : MonoBehaviour
{
    public static CashManager Instance;

    [SerializeField] private TMP_Text cashText;

    private int cash = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateCashUI();
    }

    public void AddCash(int amount)
    {
        cash += amount;
        UpdateCashUI();
    }

    public bool SpendCash(int amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            UpdateCashUI();
            return true;
        }

        return false;
    }

    public int GetCash()
    {
        return cash;
    }

    private void UpdateCashUI()
    {
        if (cashText != null)
            cashText.text = "Cash: " + cash;
    }
}