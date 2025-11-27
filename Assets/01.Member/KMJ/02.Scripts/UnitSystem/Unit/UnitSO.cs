using System;
using System.Collections.Generic;
using Skill;
using UnityEditor.Animations;
using UnityEngine;

public enum EntityType
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

    public float turnSpeed;

    public bool isPlayerUnit;

    public float turnGauge;
    
    public float moveSpeed;
    
    public float attackDistance;

    [Header("UnitType")]
    public EntityType entityType = EntityType.MeleeAttacker;


    private void OnValidate()
    {
        if (isLongRange)
            entityType = EntityType.LongRanger;
        else
            entityType = EntityType.MeleeAttacker;
    }
}
