using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    [Header("Damage Buff")]
    [SerializeField] private int damageBonus = 0;

    public int GetDamageBonus()
    {
        return damageBonus;
    }

    public void AddDamageBonus(int amount)
    {
        damageBonus += amount;
        Debug.Log("Player damage bonus is now: " + damageBonus);
    }
}