using UnityEngine;

public class AnimatorSetBoolFunction : Runnable
{
    public string boolName;
    public bool value;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public void Run(bool value)
    {
        this.value = value;
        Run();
    }

    protected override void RunInternal()
    {
        animator.SetBool(boolName, value);
    }
}