using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public void DepositGold(int amount)
    {
        GameManager.Instance.AddGold(amount);
    }
}