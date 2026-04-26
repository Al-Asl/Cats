using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 2f;

    private Vector3 dir;

    public void SetTarget(Transform t)
    {
        dir = (t.position - transform.position).normalized;
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.transform.TryGetComponent(out EnemyUnit enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}