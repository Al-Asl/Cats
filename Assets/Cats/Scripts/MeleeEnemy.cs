using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemy : EnemyUnit
{
    public UnityEvent onStop;
    public UnityEvent onWalk;

    public float damage = 2f;
    public float attackInterval = 1f;
    public float stoppingDistance = 1.2f;

    private NavMeshAgent agent;
    private float attackTimer;
    private PlayerUnit currentTarget;

    private bool isWalking;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateTarget();
        Move();
        Attack();
    }

    void UpdateTarget()
    {
        PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        float closestDist = Mathf.Infinity;
        PlayerUnit closest = null;

        foreach (var unit in units)
        {
            float dist = Vector3.Distance(transform.position, unit.transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = unit;
            }
        }

        currentTarget = closest;
    }

    void Move()
    {
        if(agent.isStopped || agent.velocity.magnitude <= 0.1f)
        {
            if (isWalking)
            {
                onStop.Invoke();
                isWalking = false;
            }
        }
        else
        {
            if (!isWalking)
            {
                onWalk.Invoke();
                isWalking = true;
            }
        }

        if (currentTarget == null) return;

        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(currentTarget.transform.position);
    }

    void Attack()
    {
        if (currentTarget == null) return;

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist <= stoppingDistance + 0.1f)
        {
            agent.isStopped = true;

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                currentTarget.TakeDamage(damage);
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }
}