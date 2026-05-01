using UnityEngine;
using System.Collections.Generic;

public class GoldMine : MonoBehaviour
{
    public static List<GoldMine> mines = new List<GoldMine>();

    private void OnEnable()
    {
        mines.Add(this);
    }

    private void OnDisable()
    {
        mines.Remove(this);
    }

    public int totalGold = 100;

    public bool HasGold()
    {
        return totalGold > 0;
    }

    public int ExtractGold(int desired)
    {
        if (totalGold <= 0)
            return 0;

        int amount = Mathf.Min(desired, totalGold);
        totalGold -= amount;

        return amount;
    }
}