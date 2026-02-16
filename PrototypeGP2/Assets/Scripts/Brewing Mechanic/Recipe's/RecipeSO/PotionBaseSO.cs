using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Crafting/Potion")]
public class PotionBaseSO : ItemScriptableObject
{
    public PotionRecipeSO recipe;

    public void OnEnable()
    {
        itemType = ItemType.Potion;
    }
}
