using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIitem : MonoBehaviour, IPointerClickHandler
{
   public Item item;
   
   //public BattleSystem battleSystem;
   //public DragManager DragManager;
   //public InventoryObject InventoryObject;
   
   
   public void Awake()
   {
    
      UpdateItem(null);
   }

   public void UpdateItem(Item item)
   {  
      this.item = item;
     
      
   }
   public void OnPointerClick(PointerEventData eventData)
   {
      if (item == null) return;

      

      //InventoryManager.Instance.UseItem(item);
      
      //Connecting it to the battlesystem
      //BattleSystem.SelectItem(item);
      //DragManager.Instance.BeginUIDrag(this);

   }

}
