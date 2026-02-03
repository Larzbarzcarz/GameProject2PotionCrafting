using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject Spine;
    public GameObject Arms;
    
    public GameObject LeftShoulder;
    
    
    public void Awake()
    {
        Spine.SetActive(false);
        Arms.SetActive(false);
        LeftShoulder.SetActive(false);
    }

    public void Evolve()
    {
        Debug.Log("Evolve");
        Spine.SetActive(true);
        Arms.SetActive(true);
        LeftShoulder.SetActive(true);
    }

    public void Devolve()
    { 
        Debug.Log("Devolve");
        Spine.SetActive(false);
        Arms.SetActive(false);
        LeftShoulder.SetActive(false);
    }
    
    
    
    
}
