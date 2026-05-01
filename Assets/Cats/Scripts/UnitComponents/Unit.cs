using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public enum Team
{
    Team1,
    Team2,
    Team3,
    Team4,
}

public interface ITeam
{
    Team Team { get; set; }
}

public static class TeamUtility
{
    public static void SetTeam(GameObject gameObject, Team team)
    {
        var teamsCompnents = gameObject.GetComponentsInChildren<ITeam>();
        foreach(var teamComp in teamsCompnents)
            teamComp.Team = team;
    }
}

public static class UnityExtensions
{
    public static float Distance(this Component component, Component other)
    {
        return Vector3.Distance(component.transform.position, other.transform.position);
    }

    public static float Distance(this Transform transform, Transform other)
    {
        return Vector3.Distance(transform.position, other.position);
    }
}

[RequireComponent(typeof(Collider))]
public class Unit : MonoBehaviour, ITeam
{
    [InfoBox("This component is the main building block for unit, isTrigger in the collider will be set to On at the start")]
    public Team team;

    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    public float maxHealth = 10f;
    [ShowNonSerializedField]
    float currentHealth;

    bool dead;
    public Team Team { get => team; set => team = value; }

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
        DeattachArt();
        onDeath.Invoke();
        Destroy(gameObject);
    }

    private void DeattachArt()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Trim().ToLower() == "art")
            {
                transform.GetChild(i).parent = null;
                break;
            }
        }
    }
}