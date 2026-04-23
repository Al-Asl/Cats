using UnityEngine;
using UnityEngine.AI;

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
        targetMine = FindObjectOfType<MineUnit>();
        baseUnit = BaseUnit.Instance;

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

    void HandleBase()
    {
        if (baseUnit == null)
            return;

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