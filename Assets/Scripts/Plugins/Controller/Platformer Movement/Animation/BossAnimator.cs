using ControllerSystem.Platformer2D;
using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    private Animator animator;
    private GroundCheck groundCheck;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        groundCheck = GetComponentInChildren<GroundCheck>();

        groundCheck.OnGrounded += HandleGrounded;
    }

    private void HandleGrounded()
    {
        animator.SetTrigger("Land");
    }
}
