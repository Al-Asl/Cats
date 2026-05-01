using UnityEngine;

public class UnitSpawner : MonoBehaviour, ITeam
{
    public Team team;

    public ObjectPicker picker;
    public SpawnConfiguration spawn;
    [SerializeReference, SubclassSelector]
    public ShapeSampler shape = new LineSampler();

    public Team Team { get => team; set => team = value; }

    private void Update()
    {
        int amount = 0;

        if (spawn.Update(out amount))
        {
            foreach (var sample in shape.Sample(TransformState.GetState(transform), amount))
            {
                var prefab = picker.GetNext();

                if (prefab != null)
                {
                    GameObject unitPrefab = Instantiate(picker.GetNext(), sample.Position, sample.Rotation);
                    unitPrefab.SetActive(true);
                    TeamUtility.SetTeam(unitPrefab, team);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        shape.OnDrawGizmos(TransformState.GetState(transform));
    }
}