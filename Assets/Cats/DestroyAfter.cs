using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public void Destroy(float time)
    {
        Destroy(gameObject, time);
    }
}
