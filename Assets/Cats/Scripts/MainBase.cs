using UnityEngine;
using System.Collections.Generic;

public class MainBase : MonoBehaviour
{
    public static Dictionary<Team, MainBase> bases = new Dictionary<Team, MainBase>();

    public Team team;
    public int gold;

    private void Awake()
    {
        bases.Add(team, this);
    }

    public void Deposit(uint amount)
    {
        gold += (int)amount;
    }
}