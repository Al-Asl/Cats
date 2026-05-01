using UnityEngine;

public class AnimatorSetStateFunction : Runnable
{
    public string stateName;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public void Run(string stateName)
    {
        this.stateName = stateName;
        Run();
    }

    protected override void RunInternal()
    {
        animator.Play(stateName, 0, 0f);
    }
}