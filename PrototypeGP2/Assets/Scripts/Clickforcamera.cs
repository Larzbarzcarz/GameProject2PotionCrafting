using _Project._Scripts.Sound_and_Music;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clickforcamera : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject craftingCamera;
    public GameObject LabUi;
    public GameObject Return;
    public GameObject Craft;
    public GameObject Materialing;
    void Start()
    {
		Materialing.SetActive(false);
        Craft.SetActive(false);
        Return.SetActive(false);
        mainCamera.SetActive(true);
        craftingCamera.SetActive(false);

        //-----FMOD Integration-----
        //AudioManager.Instance.PlayMusic(FMODEvents.instance.baseMusic);
    }

    public void SwitchToCrafting()
    {
		Materialing.SetActive(true);
        Craft.SetActive(true);
        Return.SetActive(true);
        LabUi.SetActive(false);
        Debug.Log("Switching to crafting camera");
        mainCamera.SetActive(false);
        craftingCamera.SetActive(true);

        //-----FMOD Integration-----
        //AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
    }

    public void SwitchToMain()
    {
		Materialing.SetActive(false);
        Craft.SetActive(false);
        Return.SetActive(false);
        LabUi.SetActive(true);
        Debug.Log("Switching to main camera");
        craftingCamera.SetActive(false);
        mainCamera.SetActive(true);

        //-----FMOD Integration-----
        //AudioManager.Instance.PlayMusic(FMODEvents.instance.baseMusic);
        //AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
    }

    public void SwitchToCombat()
    {
        //-----FMOD Integration-----
        //AudioManager.Instance.PlayOneShot(FMODEvents.instance.expeditionStart);
        //-----FMOD Integration-----
        SceneManager.LoadSceneAsync(1);
    }

    
}
