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

[CreateAssetMenu (fileName = "Unit", menuName = "SO/UnitSO/UnitSO")]
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
    
    [Space(4)]
    [Header("UnitSettings")]
    public bool isLongRange;

    public int turnSpeed = 3;
    
    public bool isPlayerUnit = false;
    
    public int MoveRange;

    public int Maxhealth;
    
    public int AttackDamage;

    public int DefensivePower;

    public int ShieldValue;

    [Range(0,100)]
    public float AvoidProbability;
    
    [Range(0,100)]
    public float CriticalProbability;

    public float CriticalDamageIncrease;

    [Header("SkillSystem")] 
    public int MaxSkillCost;

    public int RecoverySkillCost;

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