using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Mesh itemMesh;
    public bool isStackable = true;
}

 //public enum ItemType
 //{
 //Mushroom,
 //Bat_wing

 //}



