using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public enum DamageColliderType
{
    DPS,
    OnEnter,
    OnExit,
}

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DamageCollider : MonoBehaviour, ITeam
{
    [InfoBox("A damage dealer through collider, isKinematic in the RigidBody will be set to On at the start")]
    public Team team;
    public DamageColliderType type;
    public float amount = 1;
    public float period = 1;

    private List<Unit> inRange = new List<Unit>();

    private float counter;

    public Team Team { get => team; set => team = value; }

    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void Update()
    {
        if(type == DamageColliderType.DPS)
        {
            if(counter >= period)
            {
                inRange.RemoveAll((unit)=> unit == null || !unit.enabled);
                foreach (var unit in inRange)
                    unit.TakeDamage(amount);
                counter = 0;
            }else
                counter += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if(unit.team != team)
            {
                inRange.Add(unit);
                if (type == DamageColliderType.OnEnter)
                    unit.TakeDamage(amount);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent(out Unit unit))
        {
            if (unit.team != team)
            {
                inRange.Remove(unit);
                if (type == DamageColliderType.OnExit)
                    unit.TakeDamage(amount);
            }
        }
    }
}
