using UnityEngine;

public class RangePlayerUnit : PlayerUnit
{
    public float range = 6f;
    public float fireRate = 1f;

    public GameObject projectilePrefab;
    public Transform firePoint;

    private float fireTimer;

    private void Update()
    {
        fireTimer += Time.deltaTime;

        EnemyUnit target = FindClosestEnemy();

        if (target != null && fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Shoot(target);
        }
    }

    EnemyUnit FindClosestEnemy()
    {
        EnemyUnit[] enemies = FindObjectsOfType<EnemyUnit>();

        EnemyUnit closest = null;
        float minDist = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            if (dist < range && dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    void Shoot(EnemyUnit target)
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        proj.GetComponent<Projectile>().SetTarget(target.transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}