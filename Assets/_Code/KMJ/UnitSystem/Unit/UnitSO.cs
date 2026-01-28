using System.Collections.Generic;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using Skill;
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
    
    [Header("UnitImage")]
    public Sprite UnitImage;

    [Header("WhatItIs")] 
    public UnitInfoSO UnitInfo;
    
    [Header("OwnUnitCards")]
    public List<SkillSO> unitSkillCards;

    [Header("CharacterAnimationController")]
    public RuntimeAnimatorController animationController;

    [Space(4)]
    [Header("CharacterOwnCost")]
    public int cost;
    
    [Space(4)]
    [Header("UnitSettings")]
    public bool isLongRange;

    public float turnSpeed = 3f;

    public bool isPlayerUnit = false;
    
    public float moveSpeed;

    public float atkDamage;
    
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
