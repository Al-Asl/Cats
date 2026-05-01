using UnityEngine;

public class DestroyFunction : Runnable
{
    protected override void RunInternal()
    {
        Object.Destroy(gameObject);
    }
}