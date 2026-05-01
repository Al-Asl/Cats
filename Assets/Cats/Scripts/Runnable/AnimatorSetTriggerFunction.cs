using UnityEngine;

public class AnimatorSetTriggerFunction : Runnable
{
    public string triggerName;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void RunInternal()
    {
        animator.SetTrigger(triggerName);
    }
}