using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Turn Active", story: "Is [IsMyTurn] active", category: "Conditions", id: "940b81db8af8ff2f9c0a87f47692850b")]
public partial class IsTurnActiveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> IsMyTurn;

    public override bool IsTrue()
    {
        return IsMyTurn.Value;
    }
}
