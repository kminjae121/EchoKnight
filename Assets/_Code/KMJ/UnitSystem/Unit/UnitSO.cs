using _Code.KMJ.UnitSystem.involveUnitSO;
using Code.UnitSystem;
using Code.UnitSystem.ArtifactSystem;
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
    
    [Header("UnitClass")]
    public string UnitClass;
    
    [Header("UnitImage")]
    public Sprite UnitImage;

    [Header("UnitSpawn")]        
    public UnitSpawnSO UnitSpawn;

    [Header("SkillStorage")] public UnitSkillStorageSO SkillStorage;
    
    [Header("OwnSkillStorage")]
    public UnitOwnSkillStorageSO OwnSkillStorage;
    
    [Header("ArtifactStorage")]
    public ArtifactStorageSO OwnArtifactStorage;
    public ArtifactStorageSO EquippedArtifacts;

    [Space(4)]
    [Header("LoadOutCost")]
    public int LoadOutCost;

    [Space(3)] 
    [TextArea]public string UnitDescription;
    
    [Space(4)]
    [Header("UnitSettings")]
    public bool isLongRange;

    public float turnSpeed = 3f;

    public int moveRange;
    
    public bool isPlayerUnit = false;
    
    public float MoveSpeed;

    public float Maxhealth;

    public float AtkDamage;

    [Range(1, 3f)]
    public float SkillDamage;

    public float DefensivePower;

    [Header("UnitType")] 
    public UnitInGameSO unitInGame;
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