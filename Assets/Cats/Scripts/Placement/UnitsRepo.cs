using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[System.Serializable]
public class UnitEntity
{
    public string name;
    public int price = 10;
    public UnitPlacement prefab;
}

public class UnitsRepo : ScriptableObject
{
    public List<UnitEntity> entities;

    public UnitEntity GetUnit(GameObject prefab)
    {
        return entities.Find((unit) => unit.prefab == prefab);
    }
}