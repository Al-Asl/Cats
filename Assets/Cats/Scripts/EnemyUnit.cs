using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyUnit : MonoBehaviour
{
    public float maxHealth = 10f;
    protected float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}