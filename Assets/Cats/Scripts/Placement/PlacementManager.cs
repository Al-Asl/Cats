using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class PlacementManager : MonoBehaviour
{
    public Team playerTeam;

    public static PlacementManager Instance;

    public UnitsRepo unitsRepo;
    public LayerMask placementAreaMask;
    public int entityIndex;

    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public MainBase GetBase() { return MainBase.bases[playerTeam]; }

    public bool CanAfford(UnitEntity unit)
    {
        return GetBase().gold >= unit.price;
    }

    public void SpendGold(int amount)
    {
        GetBase().gold -= amount;
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

        var unit = unitsRepo.entities[entityIndex];

        if(unit == null)
        {
            Debug.LogError("the selected prefab wasn't found in game settings!");
            return;
        }

        PlacementArea area = GetPlacementArea(hitPoint);

        if (area == null)
            return;

        if (!area.ContainsCircle(hitPoint, unit.prefab.radius))
            return;

        if (IsOverlapping(hitPoint, unit.prefab.radius))
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
            return hits[0].GetComponent<PlacementArea>();

        return null;
    }

    bool IsOverlapping(Vector3 position, float radius)
    {
        var units = FindObjectsByType<Unit>(FindObjectsSortMode.None).
            Where((Unit) => Unit.team == playerTeam).
            Select((unit) => unit.GetComponent<UnitPlacement>()).
            Where(placement => placement);

        foreach (var unit in units)
        {
            float dist = Vector3.Distance(
                new Vector3(position.x, 0f, position.z),
                new Vector3(unit.transform.position.x, 0f, unit.transform.position.z)
            );

            float minDistance = radius + unit.radius;

            if (dist < minDistance)
                return true;
        }

        return false;
    }

    void PlaceUnit(Vector3 position)
    {
        var entity = unitsRepo.entities[entityIndex];

        if (!CanAfford(entity))
            return;

        SpendGold(entity.price);

        position.y = 0f;
        var go = Instantiate(entity.prefab, position, Quaternion.identity);
    }
}