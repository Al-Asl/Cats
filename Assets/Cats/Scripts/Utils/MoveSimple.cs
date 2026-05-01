using UnityEngine;

public class MoveSimple : MonoBehaviour
{
    public Space space = Space.Self;
    public Vector3 direction = Vector3.forward;
    public float speed = 2;

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, space);
    }
}