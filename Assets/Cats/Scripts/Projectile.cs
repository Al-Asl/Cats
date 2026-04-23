using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 2f;

    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            Hit();
        }
    }

    void Hit()
    {
        if (target.TryGetComponent(out EnemyUnit enemy))
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}