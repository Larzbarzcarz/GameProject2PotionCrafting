using UnityEngine;
using UnityEngine.SceneManagement;

public class Clickforcamera : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject craftingCamera;
    public GameObject LabUi;
    public GameObject Return;
    public GameObject Craft;
    
    
    void Start()
    {
        Craft.SetActive(false);
        Return.SetActive(false);
        mainCamera.SetActive(true);
        craftingCamera.SetActive(false);
    }

    public void SwitchToCrafting()
    {
        Craft.SetActive(true);
        Return.SetActive(true);
        LabUi.SetActive(false);
        Debug.Log("Switching to crafting camera");
        mainCamera.SetActive(false);
        craftingCamera.SetActive(true);
    }

    public void SwitchToMain()
    {
        Return.SetActive(false);
        LabUi.SetActive(true);
        Debug.Log("Switching to main camera");
        craftingCamera.SetActive(false);
        mainCamera.SetActive(true);
    }

    public void SwitchToCombat()
    {
        SceneManager.LoadSceneAsync(1);
    }

    
}
