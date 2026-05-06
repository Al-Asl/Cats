using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.Events;

public enum TriggerState
{
    OnEnter,
    OnExit,
}

public abstract class DamageDealer
{
    public UnityEvent OnDealDamage;

    public float amount = 1;

    public virtual void Update(Team team) { }

    public virtual void OnTriggerEnter(Collider other, Team team) {}

    public virtual void OnTriggerExit(Collider other, Team team) {}
}

[System.Serializable]
public class OnContactDamageDealer : DamageDealer
{
    public TriggerState type;

    public override void OnTriggerEnter(Collider other, Team team) 
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if (unit.team != team)
            {
                if (type == TriggerState.OnEnter)
                {
                    OnDealDamage.Invoke();
                    unit.TakeDamage(amount);
                }
            }
        }
    }

    public override void OnTriggerExit(Collider other, Team team) 
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if (unit.team != team)
            {
                if (type == TriggerState.OnExit)
                {
                    OnDealDamage.Invoke();
                    unit.TakeDamage(amount);
                }
            }
        }
    }
}

[System.Serializable]
public class DPSDamageDealer : DamageDealer
{
    public float period = 1;

    private List<Unit> inRange = new List<Unit>();
    private float counter;

    public override void Update(Team team)
    {
        if (counter >= period)
        {
            inRange.RemoveAll((unit) => unit == null || !unit.enabled);
            foreach (var unit in inRange)
            {
                OnDealDamage.Invoke();
                unit.TakeDamage(amount);
            }
            counter = 0;
        }
        else
            counter += Time.deltaTime;
    }

    public override void OnTriggerEnter(Collider other, Team team)
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if (unit.team != team)
                inRange.Add(unit);
        }
    }

    public override void OnTriggerExit(Collider other, Team team)
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if (unit.team != team)
                inRange.Remove(unit);
        }
    }
}

[System.Serializable]
public class AutoDamageDealer : DamageDealer
{
    public enum State
    {
        Idle,
        Damage,
        Charge,
        CoolDown
    }

    public UnityEvent OnCharge;
    public UnityEvent OnAttackEnd;
    public float chargePeriod = 1;
    public float damagePeriod = 1;
    public float coolDownPeriod = 1;

    private List<Unit> inRange = new List<Unit>();

    [ShowNonSerializedField]
    private State state;
    private float counter;

    public override void Update(Team team)
    {
        if(state == State.Charge)
        {
            if (counter >= chargePeriod)
            {
                counter = 0;
                state = State.Damage;
                inRange.RemoveAll((unit) => unit == null || !unit.enabled);
                foreach (var unit in inRange)
                {
                    OnDealDamage.Invoke();
                    unit.TakeDamage(amount);
                }
            }
            else
                counter += Time.deltaTime;
        }

        if (state == State.Damage)
        {
            if (counter >= damagePeriod)
            {
                counter = 0;
                state = State.CoolDown;
                OnAttackEnd.Invoke();
            }
            else
                counter += Time.deltaTime;
        }

        if (state == State.CoolDown)
        {
            if (counter >= coolDownPeriod)
            {
                counter = 0;
                if (inRange.Count == 0)
                    state = State.Idle;
                else
                {
                    state = State.Charge;
                    OnCharge.Invoke();
                }
            }
            else
                counter += Time.deltaTime;
        }
    }

    public override void OnTriggerEnter(Collider other, Team team)
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if (unit.team != team)
            {
                inRange.Add(unit);
                if (state == State.Idle)
                {
                    state = State.Charge;
                    OnCharge.Invoke();
                }
                else if (state == State.Damage)
                {
                    OnDealDamage.Invoke();
                    unit.TakeDamage(amount);
                }

            }
        }
    }

    public override void OnTriggerExit(Collider other, Team team)
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if (unit.team != team)
                inRange.Remove(unit);
        }
    }
}

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DamageCollider : MonoBehaviour, ITeam
{
    [InfoBox("A damage dealer through collider, isKinematic in the RigidBody will be set to On at the start")]
    public Team team;
    [SerializeReference, SubclassSelector]
    public DamageDealer damageDealer;

    public Team Team { get => team; set => team = value; }

    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Update()
    {
        damageDealer.Update(team);
    }

    private void OnTriggerEnter(Collider other)
    {
        damageDealer.OnTriggerEnter(other, team);
    }

    private void OnTriggerExit(Collider other)
    {
        damageDealer.OnTriggerExit(other, team);
    }
}