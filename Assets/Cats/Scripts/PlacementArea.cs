using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlacementArea : MonoBehaviour
{
    private BoxCollider box;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Checks if a circle (on XZ plane) is fully inside this area
    /// </summary>
    public bool ContainsCircle(Vector3 center, float radius)
    {
        Bounds bounds = box.bounds;

        return (center.x - radius >= bounds.min.x &&
                center.x + radius <= bounds.max.x &&
                center.z - radius >= bounds.min.z &&
                center.z + radius <= bounds.max.z);
    }

    /// <summary>
    /// Optional: check if a point is inside (used for raycast validation)
    /// </summary>
    public bool ContainsPoint(Vector3 point)
    {
        return box.bounds.Contains(point);
    }

    private void OnDrawGizmos()
    {
        if (box == null)
            box = GetComponent<BoxCollider>();

        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(box.center, box.size);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(box.center, box.size);
    }
}