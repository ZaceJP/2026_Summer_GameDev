using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Roguelike/Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Classification")]
    public UpgradeType upgradeType;
    public UpgradeRarity rarity;

    [Header("Class Restriction")]
    public HeroClass allowedClass = HeroClass.Universal;

    [Header("Values")]
    public float value;
    public bool isPercentage = false;
}