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

public enum UnitType
{
    None,
    Archer,
    Bandlt,
    Knight,
    Magician,
}

[CreateAssetMenu (fileName = "Unit", menuName = "UnitSO")]
public class UnitSO : ScriptableObject
{
    [Header("UnitName")]
    public string UnitName;
    
    [Header("UnitImage")]
    public Sprite UnitImage;

    [Header("WhatItIs")] 
    public UnitSpawnSO UnitSpawn;
    
    [Header("OwnUnitCards")]
    public List<SkillSO> UnitSkillCards;

    [Space(4)]
    [Header("CharacterOwnCost")]
    public int Cost;
    
    [Space(4)]
    [Header("UnitSettings")]
    public bool isLongRange;

    public float turnSpeed = 3f;

    public bool isPlayerUnit = false;
    
    public float MoveSpeed;

    public float Maxhealth;

    public float AtkDamage;
    
    public float AttackDistance;

    [Header("UnitType")]
    public EntityType EntityType = EntityType.MeleeAttacker;

    public UnitType UnitType = UnitType.None;


    private void OnValidate()
    {
        if (isLongRange)
            EntityType = EntityType.LongRanger;
        else
            EntityType = EntityType.MeleeAttacker;
    }
}
