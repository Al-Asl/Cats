using UnityEngine;

public class MineUnit : MonoBehaviour
{
    public int totalGold = 100;
    public int goldPerTrip = 10;

    public bool HasGold()
    {
        return totalGold > 0;
    }

    public int ExtractGold()
    {
        if (totalGold <= 0)
            return 0;

        int amount = Mathf.Min(goldPerTrip, totalGold);
        totalGold -= amount;

        return amount;
    }
}