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

    public void Fire(int damage, Action<Projectile> returnAction)
    {
        this.damage = damage;
        this.returnAction = returnAction;
        returnTime = Time.time + lifetime;
        alive = true;
    }

    private void Update()
    {
        if (!alive) return;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Time.time >= returnTime)
            Return();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alive) return;

        var health = other.GetComponentInParent<EnemyHealth>();
        if (health != null)
            health.TakeDamage(damage);

        Return();
    }

    private void Return()
    {
        if (!alive) return;
        alive = false;
        returnAction?.Invoke(this);
    }

    public void OnGetFromPool()
    {
        alive = false;
    }

    public void OnReturnFromPool()
    {
        returnAction = null;
    }
}
