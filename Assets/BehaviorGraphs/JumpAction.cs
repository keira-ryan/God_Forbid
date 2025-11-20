using System;
using DG.Tweening;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine.Serialization;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "[Agent] jumps [animated]", category: "Action", id: "36855be5acfb08a5e7ee442b966691ed")]
public partial class JumpAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> player;
    public float horizontalForce = 2.0f;
    public float jumpForce = 5.0f;

    public float buildupTime;
    public float jumpTime;

    public string animationTrigger;

    public Rigidbody2D body;
    public Animator animator;

    private bool hasLanded;
    private Tween buildupTween;
    private Tween jumpTween;

    protected override Status OnStart()
    {
        animator = Agent.Value.GetComponentInChildren<Animator>();
        //animator.SetTrigger("Jump");
        body = Agent.Value.GetComponent<Rigidbody2D>();
        DOVirtual.DelayedCall(buildupTime, StartJump, false);
        return Status.Running;
    }

    private void StartJump()
    {
        var direction = player.Value.transform.position.x < Agent.Value.transform.position.x ? -1 : 1;
        body.AddForce(new Vector2(horizontalForce * direction, jumpForce), ForceMode2D.Impulse);

        DOVirtual.DelayedCall(jumpTime, () =>
        {
            hasLanded = true;

        }, false);
    }
    
    protected override Status OnUpdate()
    {
        return hasLanded ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        buildupTween?.Kill();
        jumpTween?.Kill();
        hasLanded = false;
        
    }
}

