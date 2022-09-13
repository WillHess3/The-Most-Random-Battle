using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Weapon")]
public class Weapons : ScriptableObject {

    [Header("Sprite")]
    public Sprite weaponSprite;
    public Sprite playerSprite;

    public float playerXOffset;

    [Header("Weapon Info")]
    public bool isMelee;
    public float attackRadius;
    public int damage;

    [Space]
    [Range(0, 1)] public float chestMeanHitChance;
    [Range(0, 1)] public float chestMeanHitChanceMaxDistance;
    [Range(0, 1)] public float chestHitChanceStandardDeviation;
    
    [Space]
    [Range(0, 1)] public float limbMeanHitChance;
    [Range(0, 1)] public float limbMeanHitChanceMaxDistance;
    [Range(0, 1)] public float limbHitChanceStandardDeviation;
    
    [Space]
    [Range(0, 1)] public float headMeanHitChance;
    [Range(0, 1)] public float headMeanHitChanceMaxDistance;
    [Range(0, 1)] public float headHitChanceStandardDeviation;

    [Space]
    public int durability;
}
