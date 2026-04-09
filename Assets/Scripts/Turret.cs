using UnityEngine;
using StarterAssets;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile projectilePrefab;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Combat")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private int damage = 1;

    [Header("Pool")]
    [SerializeField] private int poolSize = 10;

    private ObjectPool<Projectile> projectilePool;
    private Transform player;
    private float nextFireTime;

    private void Start()
    {
        player = FindAnyObjectByType<FirstPersonController>().transform;
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, transform, poolSize);
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRange) return;

        TrackPlayer();

        if (Time.time >= nextFireTime)
            Fire();
    }

    private void TrackPlayer()
    {
        Vector3 direction = (player.position - head.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        head.rotation = Quaternion.Slerp(head.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Fire()
    {
        nextFireTime = Time.time + (1f / fireRate);

        Projectile projectile = projectilePool.Get(firePoint.position, firePoint.rotation);
        projectile.Fire(damage, ReturnProjectile);
    }

    private void ReturnProjectile(Projectile projectile)
    {
        projectilePool.Return(projectile);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
