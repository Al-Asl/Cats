using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class ChaserAgent : MonoBehaviour, ITeam
{
    public UnityEvent onStop;
    public UnityEvent onWalk;

    public Team team;
    public float stoppingDistance = 1.2f;

    private NavMeshAgent agent;
    private Unit target;
    private bool isWalking;
    public Team Team { get => team; set => team = value; }

    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        UpdateTarget();
        Move();

        if (agent.isStopped || agent.velocity.magnitude <= 0.1f)
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
    }

    void UpdateTarget()
    {
        var units = FindObjectsByType<Unit>(FindObjectsSortMode.None).
            Where((unit)=> unit.team != team);

        float closestDist = Mathf.Infinity;
        Unit closest = null;

        foreach (var unit in units)
        {
            float dist = Vector3.Distance(transform.position, unit.transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = unit;
            }
        }

        target = closest;
    }

    void Move()
    {
        if (target == null) return;

        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(target.transform.position);
    }
}