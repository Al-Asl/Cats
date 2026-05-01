using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerAgent : MonoBehaviour, ITeam
{
	private enum State
	{
		ToMine,
		ToBase,
		Idle
	}

	public UnityEvent onStop;
	public UnityEvent onWalk;

	public Team team;
	public int carriedGold = 0;
	public int goldPerTrip;
	public float interactDistance = 1.2f;

	private NavMeshAgent agent;

	private MainBase mainBase;
	private GoldMine goldMine;

	private State state;
	private bool isWalking;
	public Team Team { get => team; set => team = value; }

	protected void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
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

	void HandleIdle()
	{
		if (carriedGold > 0)
		{
			mainBase = MainBase.bases[team];

			if (mainBase != null)
				state = State.ToBase;
		}
		else
		{
			var mines = FindObjectsByType<GoldMine>(FindObjectsSortMode.None);
			goldMine = mines.Aggregate((a, b) => { return transform.Distance(a) < transform.Distance(b) ? a : b; });

			if (goldMine != null)
				state = State.ToMine;
		}
	}

	void HandleMine()
	{
		if (goldMine == null || !goldMine.HasGold())
		{
			state = State.Idle;
			return;
		}

		agent.isStopped = false;
		agent.SetDestination(goldMine.transform.position);

		float dist = Vector3.Distance(transform.position, goldMine.transform.position);

		if (dist <= interactDistance)
		{
			carriedGold = goldMine.ExtractGold(goldPerTrip);
			state = State.ToBase;
		}
	}

	void HandleBase()
	{
		if (mainBase == null)
		{
			state = State.Idle;
			return;
		}

		agent.isStopped = false;
		agent.SetDestination(mainBase.transform.position);

		float dist = Vector3.Distance(transform.position, mainBase.transform.position);

		if (dist <= interactDistance)
		{
			mainBase.Deposit((uint)carriedGold);
			carriedGold = 0;

			state = State.ToMine;
		}
	}
}