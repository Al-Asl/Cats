using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlacementArea : MonoBehaviour
{
    private BoxCollider box;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    public bool ContainsCircle(Vector3 center, float radius)
    {
        Bounds bounds = box.bounds;

        return (center.x - radius >= bounds.min.x &&
                center.x + radius <= bounds.max.x &&
                center.z - radius >= bounds.min.z &&
                center.z + radius <= bounds.max.z);
    }
}