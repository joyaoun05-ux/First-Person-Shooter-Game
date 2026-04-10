using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private ActiveWeapon activeWeapon;

    private StarterAssetsInputs inputs;
    private Weapon[] allWeapons;
    private List<Weapon> unlockedWeapons = new List<Weapon>();

    private int currentIndex = -1;

    private void Awake()
    {
        inputs = GetComponentInParent<StarterAssetsInputs>();
        allWeapons = GetComponentsInChildren<Weapon>(true);
    }

    private void Start()
    {
        foreach (Weapon weapon in allWeapons)
        {
            weapon.gameObject.SetActive(false);
            Debug.Log("Found weapon: " + weapon.name + " using data: " + weapon.Data.name);
        }

        if (allWeapons.Length > 0)
        {
            UnlockWeapon(allWeapons[0]);
        }
    }

    private void Update()
    {
        if (inputs == null) return;
        if (!inputs.switchWeapon || unlockedWeapons.Count < 2) return;

        inputs.SwitchWeaponInput(false);

        int nextIndex = (currentIndex + 1) % unlockedWeapons.Count;
        EquipWeapon(nextIndex);
    }

    public void UnlockWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("UnlockWeapon called with NULL weapon");
            return;
        }

        Debug.Log("UnlockWeapon called for: " + weapon.name);

        if (!unlockedWeapons.Contains(weapon))
        {
            unlockedWeapons.Add(weapon);
            Debug.Log("Unlocked new weapon: " + weapon.name);
        }

        EquipWeapon(unlockedWeapons.IndexOf(weapon));
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= unlockedWeapons.Count)
        {
            Debug.LogError("EquipWeapon index out of range: " + index);
            return;
        }

        foreach (Weapon weapon in allWeapons)
        {
            weapon.gameObject.SetActive(false);
        }

        currentIndex = index;
        Weapon selectedWeapon = unlockedWeapons[currentIndex];
        selectedWeapon.gameObject.SetActive(true);

        Debug.Log("Equipped weapon: " + selectedWeapon.name);

        if (activeWeapon != null)
        {
            activeWeapon.SwitchWeapon(selectedWeapon);
        }
        else
        {
            Debug.LogError("ActiveWeapon reference is missing in WeaponSwitcher");
        }
    }

    public void SwitchToWeaponData(WeaponData weaponData)
    {
        if (weaponData == null)
        {
            Debug.LogError("SwitchToWeaponData received NULL WeaponData");
            return;
        }

        Debug.Log("Trying to switch to WeaponData: " + weaponData.name);

        Weapon matchingWeapon = FindWeaponByData(weaponData);

        if (matchingWeapon == null)
        {
            Debug.LogError("No matching Weapon found for WeaponData: " + weaponData.name);
            return;
        }

        UnlockWeapon(matchingWeapon);
    }

    private Weapon FindWeaponByData(WeaponData weaponData)
    {
        foreach (Weapon weapon in allWeapons)
        {
            if (weapon == null)
                continue;

            Debug.Log("Checking weapon object: " + weapon.name + " with data: " + weapon.Data.name);

            if (weapon.Data == weaponData)
            {
                Debug.Log("Matched WeaponData " + weaponData.name + " to weapon object " + weapon.name);
                return weapon;
            }
        }

        Debug.LogError("No matching Weapon found for WeaponData: " + weaponData.name);
        return null;
    }
}