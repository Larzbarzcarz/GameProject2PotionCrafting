using System;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialCollision : MonoBehaviour
{
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Pot")
        {
            if (other.gameObject != null)
            {
              
                Destroy(other.gameObject);
            }
        }
    }
}
