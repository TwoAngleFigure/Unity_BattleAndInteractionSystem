using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data New", menuName = "SO/Weapon Data")]
public class WeaponData_Base : ScriptableObject
{
    [Header("Name And Description")]
    public string weaponName;
    public string description;

    [Header("Type")]
    public WeaponType weaponType;
    public WeaponRangeType weaponRangeType;

    [Header("Damage")]
    public int damage;

    [Header("Speed")]
    public AnimationClip attackMotionClip;
    [Range(0, 100)] public int _hitboxPreDelay = 30;
    [Range(0, 100)] public int _hitboxDuration = 70;

    [Header("Long Range Data")]
    public LongAttackType longAttackType;

    public GameObject projectilePrefab;
    public GameObject hitEffectPrefab;

    public float shotRadius = 0f;

    public float aimRange = 100f;
    public float shotRange = 100f;
    public float muzzleBlockRadius = 0.1f;
}

public enum WeaponType
{
    OneHandedSword,
    TwoHandedSword,
    Axe,

    Bow,
    Crossbow,
    Staff,
}

public enum WeaponRangeType
{
    None,
    Short,
    Long,
    //Both,
}

public enum LongAttackType
{
    None,
    Hitscan,
    Projectile,
}
