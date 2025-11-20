using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FacePlayer", story: "[Self] faces [Player]", category: "Action", id: "3dcfc0ecbf4b9ea4bb6e31deb770f13f")]
public partial class FacePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private float baseScaleX;

    protected override Status OnStart()
    {
        baseScaleX = Self.Value.transform.localScale.x;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var scale = Self.Value.transform.localScale;
        scale.x = Self.Value.transform.localScale.x > Player.Value.transform.localScale.x ? -baseScaleX : baseScaleX;
        Self.Value.transform.localScale = scale;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

