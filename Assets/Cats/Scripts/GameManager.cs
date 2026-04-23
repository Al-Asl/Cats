using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Placement")]
    public PlayerUnit selectedUnitPrefab;
    public LayerMask placementAreaMask;

    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceUnit();
        }
    }

    void TryPlaceUnit()
    {
        if (!RaycastToGround(out Vector3 hitPoint))
            return;

        float radius = selectedUnitPrefab.GetRadius();

        PlacementArea area = GetPlacementArea(hitPoint);

        if (area == null)
            return;

        if (!area.ContainsCircle(hitPoint, radius))
            return;

        if (IsOverlapping(hitPoint, radius))
            return;

        PlaceUnit(hitPoint);
    }

    bool RaycastToGround(out Vector3 hitPoint)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementAreaMask))
        {
            hitPoint = hit.point;
            return true;
        }

        hitPoint = Vector3.zero;
        return false;
    }

    PlacementArea GetPlacementArea(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, 0.1f, placementAreaMask);

        if (hits.Length > 0)
        {
            return hits[0].GetComponent<PlacementArea>();
        }

        return null;
    }

    bool IsOverlapping(Vector3 position, float radius)
    {
        PlayerUnit[] units = FindObjectsByType<PlayerUnit>(FindObjectsSortMode.None);

        foreach (var unit in units)
        {
            float dist = Vector3.Distance(
                new Vector3(position.x, 0f, position.z),
                new Vector3(unit.transform.position.x, 0f, unit.transform.position.z)
            );

            float minDistance = radius + unit.GetRadius();

            if (dist < minDistance)
                return true;
        }

        return false;
    }

    void PlaceUnit(Vector3 position)
    {
        // Lock Y to ground level (important!)
        position.y = 0f;

        Instantiate(selectedUnitPrefab, position, Quaternion.identity);
    }
}