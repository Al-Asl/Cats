using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public static BaseUnit Instance;

    public int storedGold = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void DepositGold(int amount)
    {
        storedGold += amount;
    }
}