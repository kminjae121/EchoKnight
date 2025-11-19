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

    [Space(3)]
    [Header("CharacterMesh")]
    public Mesh characterMesh;
    
    [Header("CharacterMaterial")]
    public Material characterMaterial;
    
    [Header("CharacterAnimationController")]
    public AnimatorController animationController;
    
    [Header("WeaponMesh")] 
    public List<Mesh> weaponMesh;

    [Space(4)]
    [Header("CharacterOwnCost")]
    public int cost;
    
    [Space(4)]
    
    [Header("UnitSettings")]
    public bool isUseDoubleWeapon;
    public bool isLongRange;


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
