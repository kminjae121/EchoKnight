using System;
using System.Collections.Generic;
using Skill;
using UnityEditor.Animations;
using UnityEngine;

public enum UnitType
{
    LongRanger,
    MeleeAttacker,
}
[CreateAssetMenu (fileName = "Unit", menuName = "UnitSO")]
public class UnitSO : ScriptableObject
{
    [Header("UnitName")]
    public string UnitName;
    
    [Header("OwnUnitCards")]
    public List<UnitSkillCardSO> unitSkillCards;

    [Header("CharacterAnimationController")]
    public AnimatorController animationController;

    [Space(4)]
    [Header("CharacterOwnCost")]
    public int cost;
    
    [Space(4)]
    [Header("UnitSettings")]
    public bool isLongRange;

    public float moveSpeed;

    [Header("UnitType")]
    public UnitType unitType = UnitType.MeleeAttacker;


    private void OnValidate()
    {
        if (isLongRange)
            unitType = UnitType.LongRanger;
        else
            unitType = UnitType.MeleeAttacker;
    }
}
