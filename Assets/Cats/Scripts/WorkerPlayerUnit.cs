using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerPlayerUnit : PlayerUnit
{
    private enum State
    {
        ToMine,
        ToBase,
        Idle
    }

    [Header("Carry")]
    public int carriedGold = 0;

    [Header("Settings")]
    public float interactDistance = 1.5f;

    private NavMeshAgent agent;
    private State state;

    private MineUnit targetMine;
    private BaseUnit baseUnit;

    private void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        // IMPORTANT for 3D XZ plane
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        state = State.ToMine;
    }

    private void Update()
    {
        switch (state)
        {
            case State.ToMine:
                HandleMine();
                break;

            case State.ToBase:
                HandleBase();
                break;

            case State.Idle:
                HandleIdle();
                break;
        }
    }

    void HandleIdle()
    {
        if(carriedGold > 0)
        {
            var bases = FindObjectsByType<BaseUnit>(FindObjectsSortMode.None);
            baseUnit = bases.Aggregate((a, b) => { return Distance(a) < Distance(b) ? a : b; });

            if (baseUnit != null)
                state = State.ToBase;
        }
        else
        {
            var mines = FindObjectsByType<MineUnit>(FindObjectsSortMode.None);
            targetMine = mines.Aggregate((a, b) => { return Distance(a) < Distance(b) ? a : b; });

            if (targetMine != null)
                state = State.ToMine;
        }
    }

    void HandleMine()
    {
        if (targetMine == null || !targetMine.HasGold())
        {
            state = State.Idle;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(targetMine.transform.position);

        float dist = Vector3.Distance(transform.position, targetMine.transform.position);

        if (dist <= interactDistance)
        {
            carriedGold = targetMine.ExtractGold();
            state = State.ToBase;
        }
    }

    private float Distance(Component other)
    {
        return Vector3.Distance(transform.position, other.transform.position);
    }

    void HandleBase()
    {
        if (baseUnit == null)
        {
            state = State.Idle;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(baseUnit.transform.position);

        float dist = Vector3.Distance(transform.position, baseUnit.transform.position);

        if (dist <= interactDistance)
        {
            baseUnit.DepositGold(carriedGold);
            carriedGold = 0;

            state = State.ToMine;
        }
    }
}