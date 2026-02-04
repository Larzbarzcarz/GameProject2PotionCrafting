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

    //-----FMOD Integration-----
    private MusicManager Sounds;
    void Start()
    {
		Materialing.SetActive(false);
        Craft.SetActive(false);
        Return.SetActive(false);
        mainCamera.SetActive(true);
        craftingCamera.SetActive(false);

        //-----FMOD Integration-----
        Sounds = FindFirstObjectByType<MusicManager>();
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
        Sounds.PlaySound(0);
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
        Sounds.ChangeMusic(0);
        Sounds.PlaySound(0);
    }

    public void SwitchToCombat()
    {
        //-----FMOD Integration-----
        Sounds.PlaySound(3);
        Sounds.StopMusic();
        //-----FMOD Integration-----


        SceneManager.LoadSceneAsync(1);
    }

    
}
