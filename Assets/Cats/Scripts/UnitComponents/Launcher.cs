using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

public enum LoopType
{
    Once,
    Loop,
    BackAndForth
}


[System.Serializable]
public class LoopPlayer
{
    public enum AdvanceState
    {
        Normal,
        AtBoundary,
        Finished
    }

    public LoopType loopType;

    private int index = -1;
    private bool isReversing;

    public int GetIndex()
    {
        return index;
    }

    public AdvanceState Advance(int count)
    {
        AdvanceState state = AdvanceState.Normal;

        if (index == count - 2)
            state = AdvanceState.AtBoundary;

        if (loopType == LoopType.Once)
        {
            if (index < count - 1)
                index++;
            else
                state = AdvanceState.Finished;
        }
        else if (loopType == LoopType.Loop)
        {
            index = (index + 1) % count;
        }
        else if (loopType == LoopType.BackAndForth)
        {
            if (!isReversing)
            {
                if (index < count - 1)
                    index++;
                else
                {
                    isReversing = true;
                    index--;
                }
            }
            else
            {
                if (index == 1)
                   state = AdvanceState.AtBoundary;

                if (index > 0)
                    index--;
                else
                {
                    isReversing = false;
                    index++;
                }
            }
        }

        return state;
    }
}

public struct TransformState
{
    public static TransformState GetState(Transform transform) { 
        return new TransformState() { Position = transform.position, Rotation = transform.rotation };
    }

    public Vector3 Position;
    public Quaternion Rotation;
}

[System.Serializable]
public class ObjectPicker
{
    [System.Serializable]
    public class Entry
    {
        public GameObject gameObject;
        public int count = 1;
    }

    public LoopPlayer loop;
    public bool random;
    public List<Entry> entries = new List<Entry>() { new Entry() };

    private int count = 0;

    public GameObject GetNext()
    {
        if (!random)
        {
            if (count <= 0)
            {
                var state = loop.Advance(entries.Count);
                if (state == LoopPlayer.AdvanceState.Finished)
                    return null;
                else
                    count = entries[loop.GetIndex()].count;
            }

            count--;
            return entries[loop.GetIndex()].gameObject;
        }
        else
            return entries[PickIndexByRatio(entries.Select((e) => (float)e.count))].gameObject;
    }

    public static int PickIndexByRatio(IEnumerable<float> ratios)
    {
        float total = ratios.Sum();
        float random = Random.Range(0f, total);
        float accumulated = 0f;

        int i = 0;
        foreach (var r in ratios)
        {
            accumulated += r;
            if (random < accumulated)
                return i;
            i++;
        }

        return i - 1;
    }
}

public abstract class ShapeSampler
{
    public abstract IEnumerable<TransformState> Sample(TransformState state ,int count);
    public abstract void OnDrawGizmos(TransformState state);
}

[System.Serializable]
public class LineSampler : ShapeSampler
{
    public bool random;
    public float width = 1;
    public float angle;

    public override IEnumerable<TransformState> Sample(TransformState state, int count)
    {
        var leftRot = state.Rotation * Quaternion.AngleAxis(-angle, Vector3.up);
        var rightRot = state.Rotation * Quaternion.AngleAxis(angle, Vector3.up);

        Matrix4x4 mat = Matrix4x4.TRS(state.Position, state.Rotation, Vector3.one);
        var left = mat.MultiplyPoint3x4(Vector3.left * width * 0.5f);
        var right = mat.MultiplyPoint3x4(Vector3.right * width * 0.5f);

        var step = 1f / count;
        var c = step * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var t = random ? (Random.value - 0.5f) * step + c : c;

            yield return new TransformState() { 
                Position = Vector3.Lerp(left, right, t), 
                Rotation =  t < 0.5f ? Quaternion.Slerp(leftRot, state.Rotation, t * 2f) : 
                Quaternion.Slerp(state.Rotation, rightRot, (t - 0.5f) * 2f)
            };

            c += step;
        }
    }

    public override void OnDrawGizmos(TransformState state)
    {
        Matrix4x4 mat = Matrix4x4.TRS(state.Position, state.Rotation, Vector3.one);
        var left = mat.MultiplyPoint3x4(Vector3.left * width * 0.5f);
        var right = mat.MultiplyPoint3x4(Vector3.right * width * 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(left, right);

        foreach (var sample in Sample(state,5))
            Gizmos.DrawLine(sample.Position, sample.Position + sample.Rotation * Vector3.forward);
    }
}

[System.Serializable]
public class SpawnConfiguration
{
    [System.Serializable]
    public class Burst
    {
        public int amount = 1;
        public float delay = 1f;
    }

    public UnityEvent OnRoundComplete;

    public LoopPlayer loop;
    public List<Burst> bursts = new List<Burst>() { new Burst() };

    private float waitTimer = 0f;

    public bool Update(out int amount)
    {
        amount = 0;

        waitTimer += Time.deltaTime;
        var firstRound = loop.GetIndex() < 0;

        if (firstRound || waitTimer >= bursts[loop.GetIndex()].delay)
        {
            var state = loop.Advance(bursts.Count);

            if (state != LoopPlayer.AdvanceState.Finished)
            {
                if (state == LoopPlayer.AdvanceState.AtBoundary)
                    OnRoundComplete.Invoke();

                waitTimer = 0f;
                amount = bursts[loop.GetIndex()].amount;
                return true;
            }
        }

        return false;
    }
}

public class Launcher : MonoBehaviour, ITeam
{
    public Team team;
    public float range = 6f;

    public ObjectPicker picker;
    public SpawnConfiguration spawn;
    [SerializeReference, SubclassSelector]
    public ShapeSampler shape = new LineSampler();

    public Team Team { get => team; set => team = value; }

    int amount;

    private void Update()
    {
        if(amount > 0 || spawn.Update(out amount))
        {
            var target = GetTarget();

            if(target != null)
            {
                transform.LookAt(target.transform, Vector3.up);

                foreach (var sample in shape.Sample(TransformState.GetState(transform), amount))
                {
                    var prefab = picker.GetNext();

                    if (prefab != null)
                    {
                        GameObject projectile = Instantiate(picker.GetNext(), sample.Position, sample.Rotation);
                        projectile.SetActive(true);
                        TeamUtility.SetTeam(projectile, team);
                    }
                }
                amount = 0;
            }
        }
    }

    private Unit GetTarget()
    {
        return Physics.OverlapSphere(transform.position, range).
            Select((col) => col.GetComponent<Unit>()).
            Where((unit) => unit && unit.team != team).DefaultIfEmpty(null).
            Aggregate((a, b) => a.Distance(this) < b.Distance(this) ? a : b);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
        shape.OnDrawGizmos(TransformState.GetState(transform));
    }
}