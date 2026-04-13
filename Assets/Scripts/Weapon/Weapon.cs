using UnityEngine;
using StarterAssets;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip reloadSound;

    private CinemachineImpulseSource impulseSource;

    private int currentAmmo;

    public int CurrentAmmo => currentAmmo;
    public bool HasAmmo => currentAmmo > 0;
    public WeaponData Data => weaponData;

    private void Awake()
    {
        currentAmmo = weaponData.maxAmmo;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shoot()
    {
        if (!HasAmmo) return;

        currentAmmo--;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (impulseSource != null)
            impulseSource.GenerateImpulse();
    }

    public void RefillAmmo()
    {
        currentAmmo = weaponData.maxAmmo;

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }
}