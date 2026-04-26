using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnemyUnit : MonoBehaviour
{
    public float maxHealth = 10f;
    protected float currentHealth;

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    protected bool dead;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        onTakeDamage.Invoke();

        if (currentHealth <= 0 && !dead)
            Die();
    }

    protected virtual void Die()
    {
        dead = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name.Trim().ToLower() == "art")
            {
                transform.GetChild(i).parent = null;
                break;
            }
        }
        onDeath.Invoke();
        Destroy(gameObject);
    }
}