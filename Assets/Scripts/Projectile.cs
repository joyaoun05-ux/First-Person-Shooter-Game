using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;

    private int damage;
    private float returnTime;
    private bool alive;
    private Action<Projectile> returnAction;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire(int damage, Action<Projectile> returnAction)
    {
        this.damage = damage;
        this.returnAction = returnAction;
        returnTime = Time.time + lifetime;
        alive = true;

        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }

    private void Update()
    {
        if (!alive) return;

        if (Time.time >= returnTime)
            Return();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alive) return;

        EnemyHealth health = other.GetComponentInParent<EnemyHealth>();
        if (health != null)
        {
            Vector3 hitDirection = transform.forward;
            health.TakeDamage(damage, hitDirection);
        }

        Return();
    }

    private void Return()
    {
        if (!alive) return;

        alive = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        returnAction?.Invoke(this);
    }

    public void OnGetFromPool()
    {
        alive = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void OnReturnFromPool()
    {
        returnAction = null;
    }
}