using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Crafting/Potion")]
public class PotionBaseSO : ItemScriptableObject
{
    public void Awake()
    {
        itemType = ItemType.Potion;
    }
}
