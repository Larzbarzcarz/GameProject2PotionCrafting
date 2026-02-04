using UnityEngine;

public class Evolution : MonoBehaviour
{
    public GameObject Spine;
    public GameObject Arms;
    
    public GameObject LeftShoulder;

    public GameObject Mushroomneck;
    public GameObject MushroomLeftShoulder;
    public GameObject MushroomRightShoulder;

    public GameObject SecoundHead;
    
    
    public void Awake()
    {
        Spine.SetActive(false);
        Arms.SetActive(false);
        LeftShoulder.SetActive(false);
        Mushroomneck.SetActive(false);
        MushroomLeftShoulder.SetActive(false);
        MushroomRightShoulder.SetActive(false);
        SecoundHead.SetActive(false);
    }

    public void Evolve()
    {
        Debug.Log("Evolve");
        Spine.SetActive(true);
        Arms.SetActive(true);
        LeftShoulder.SetActive(true);
        Mushroomneck.SetActive(true);
        MushroomLeftShoulder.SetActive(true);
        MushroomRightShoulder.SetActive(true);
        SecoundHead.SetActive(true);
    }

    public void Devolve()
    { 
        Debug.Log("Devolve");
        Spine.SetActive(false);
        Arms.SetActive(false);
        LeftShoulder.SetActive(false);
    }
    
    
    
    
}
