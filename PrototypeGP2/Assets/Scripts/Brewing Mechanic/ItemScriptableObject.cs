using UnityEngine;


[CreateAssetMenu(fileName = "item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemScriptableObject : ScriptableObject
{

public string ItemName;
public Sprite itemSprite;
public ItemType itemType;
}
//public enum ItemType
//{
//Mushroom
//}


