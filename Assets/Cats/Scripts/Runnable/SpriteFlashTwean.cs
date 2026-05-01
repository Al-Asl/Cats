using UnityEngine;
using System.Collections;

public enum RunPoint
{
    None,
    Start,
    OnEnable
}

[System.Serializable]
public class RunConfiguration
{
    public RunPoint runPoint;
    public float delay;

    public void OnStart(MonoBehaviour mono, System.Action run)
    {
        if(runPoint == RunPoint.Start)
            Run(mono, run);
    }

    public void OnEnable(MonoBehaviour mono, System.Action run)
    {
        if (runPoint == RunPoint.OnEnable)
            Run(mono, run);
    }

    public void Run(MonoBehaviour mono, System.Action run)
    {
        if (delay > 0)
            mono.StartCoroutine(Delay(run));
        else
            run.Invoke();
    }

    private IEnumerator Delay(System.Action run)
    {
        yield return new WaitForSeconds(delay);
        run?.Invoke();
    }
}

public abstract class Runnable : MonoBehaviour
{
    public RunConfiguration runConfig;

    protected virtual void Start()
    {
        runConfig.OnStart(this, RunInternal);
    }

    protected virtual void OnEnable()
    {
        runConfig.OnEnable(this, RunInternal);
    }

    public void Run()
    {
        runConfig.Run(this, RunInternal);
    }

    protected abstract void RunInternal();
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlashTwean : Runnable
{
    public Color color = Color.red;
    public float speed = 3;
    public float time = 1f;

    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void RunInternal()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    // This is equivalent to InOutFlash with period set to one https://dotween.demigiant.com/documentation.php
    IEnumerator Flash()
    {
        float current = 0;

        while (current < time)
        {
            float t = current / time;

            float alpha = (Mathf.Sin((current * speed - 0.5f) * Mathf.PI) + 1) * 0.5f;
            alpha = alpha * Mathf.SmoothStep(0,1,(1f - t));

            spriteRenderer.color = Color.Lerp(Color.white, color, alpha);

            yield return null;
            current += Time.deltaTime;
        }

        spriteRenderer.color = Color.white;
    }
}