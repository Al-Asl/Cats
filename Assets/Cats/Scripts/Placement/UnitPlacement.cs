using UnityEngine;

public class UnitPlacement : MonoBehaviour
{
    public float radius;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}