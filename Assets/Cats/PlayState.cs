using UnityEngine;

public class PlayState : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Play(string stateName)
    {
        animator.Play(stateName, 0, 0f);
    }
}