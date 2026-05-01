using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    public Vector3 forward = Vector3.forward;
    public Vector3 up = Vector3.up;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(forward, up);
    }
}
