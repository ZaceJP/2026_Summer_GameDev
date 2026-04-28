using UnityEngine;

[CreateAssetMenu(fileName = "HeroSelection", menuName = "Game/Hero Selection")]
public class HeroSelection : ScriptableObject
{
    public HeroDefinition selectedHero;
}