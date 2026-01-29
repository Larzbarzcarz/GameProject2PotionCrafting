using UnityEngine;

public class Clickforcamera : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject craftingCamera;

    void Start()
    {
        mainCamera.SetActive(true);
        craftingCamera.SetActive(false);
    }

    public void SwitchToCrafting()
    {
        Debug.Log("Switching to crafting camera");
        mainCamera.SetActive(false);
        craftingCamera.SetActive(true);
    }

    public void SwitchToMain()
    {
        Debug.Log("Switching to main camera");
        craftingCamera.SetActive(false);
        mainCamera.SetActive(true);
    }
}