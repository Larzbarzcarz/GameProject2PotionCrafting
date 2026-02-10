using UnityEngine;

public class InventoryClickLogic : MonoBehaviour
{
    public GameObject MoveUI;
   

    public void OnClick()
    {
            MoveUI.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }








}
