using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerUnit : MonoBehaviour
{
    public float placementRadius = 0.5f;

    [Header("Health")]
    public float maxHealth = 20f;
    private float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public float GetRadius()
    {
        return placementRadius;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, placementRadius);
    }
}