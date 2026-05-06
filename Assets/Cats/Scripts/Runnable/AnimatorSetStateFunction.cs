using UnityEngine;
using System.Collections.Generic;

public class AnimatorSetStateFunction : Runnable
{
    public List<string> ifNot;

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
        foreach (var name in ifNot)
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(name))
                return;
        animator.Play(stateName, 0, 0f);
    }
}