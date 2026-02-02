using UnityEngine;

[CreateAssetMenu(fileName = "PotionIconLibrary", menuName = "Potions/Icon Library")]
public class PotionIconLibrary : ScriptableObject
{
    [Header("Potion Icons")]
    public Sprite healIcon;
    public Sprite damageIcon;
    public Sprite mysteryIcon;

    public Sprite GetIcon(PotionEffectType effect)
    {
        return effect switch
        {
            PotionEffectType.Heal => healIcon,
            PotionEffectType.Damage => damageIcon,
            PotionEffectType.Mystery => mysteryIcon
        };
    }
}
