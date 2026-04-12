using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    private Weapon currentWeapon;

    private StarterAssetsInputs inputs;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField] private HitMarkerUI hitMarkerUI;

    private InputAction shootAction;
    private FirstPersonController controller;
    private PlayerBuffs playerBuffs;

    private const string SHOOT_ANIMATION_TRIGGER = "Shoot";
    private float nextFireTime = 0f;

    public Weapon CurrentWeapon => currentWeapon;

    private void Awake()
    {
        currentWeapon = null;
        inputs = GetComponentInParent<StarterAssetsInputs>();
        shootAction = GetComponentInParent<PlayerInput>().actions["Shoot"];
        controller = GetComponentInParent<FirstPersonController>();
        playerBuffs = GetComponentInParent<PlayerBuffs>();

        if (hitMarkerUI == null)
        {
            hitMarkerUI = FindObjectOfType<HitMarkerUI>(true);
        }
    }

    private void Update()
    {
        HandleShoot();
    }

    private void HandleShoot()
    {
        bool canFire = Time.time >= nextFireTime;

        if (!canFire || currentWeapon == null || !currentWeapon.HasAmmo)
            return;

        if (weaponData.isAutomatic)
        {
            if (!shootAction.IsPressed()) return;
        }
        else
        {
            if (!inputs.shoot) return;
            inputs.ShootInput(false);
        }

        nextFireTime = Time.time + (1.0f / weaponData.fireRate);

        animator.Play(SHOOT_ANIMATION_TRIGGER, 0, 0f);
        currentWeapon.Shoot();

        float yawKick = Random.Range(-weaponData.recoilX, weaponData.recoilX);
        controller.ApplyRecoil(weaponData.recoilY, yawKick);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, weaponData.range))
        {
            EnemyHealth health = hit.collider.GetComponentInParent<EnemyHealth>();

            if (health != null)
            {
                int finalDamage = weaponData.damage;

                if (playerBuffs != null)
                {
                    finalDamage += playerBuffs.GetDamageBonus();
                }

                bool wasKill = health.TakeDamage(finalDamage);

                if (hitMarkerUI != null)
                {
                    hitMarkerUI.ShowHitMarker(wasKill);
                }
            }

            if (weaponData.hitVFXPrefab != null)
            {
                Instantiate(weaponData.hitVFXPrefab, hit.point, Quaternion.identity);
            }
            else if (hitFXPrefab != null)
            {
                Instantiate(hitFXPrefab, hit.point, Quaternion.identity);
            }
        }
    }

    public void SwitchWeapon(Weapon newWeapon)
    {
        if (newWeapon == null) return;

        currentWeapon = newWeapon;
        weaponData = newWeapon.Data;
        nextFireTime = 0f;
        inputs.ShootInput(false);
    }
}