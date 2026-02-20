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

    public GameObject CraftingInventory;
    public GameObject CraftingButton;
	public GameObject Recipes;
	public GameObject Results;

    public TouchManager touchManager;
    void Start()
    {
		Materialing.SetActive(false);
        Craft.SetActive(false);
        Return.SetActive(false);
        mainCamera.SetActive(true);
        craftingCamera.SetActive(false);
		Recipes.SetActive(false);
		Results.SetActive(false);

        //-----FMOD Integration-----
        touchManager = FindFirstObjectByType<TouchManager>();

        AudioManager.Instance.PlayMusic(FMODEvents.instance.baseMusic);
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
		Results.SetActive(true);
        //-----FMOD Integration-----
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        touchManager.weCooking = false;
        //AudioManager.Instance.PlayMusic(FMODEvents.instance.cookMusic);
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
		Results.SetActive(false);

        //-----FMOD Integration-----
        AudioManager.Instance.PlayMusic(FMODEvents.instance.baseMusic);
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
    }

    public void SwitchToCombat()
    {
        //-----FMOD Integration-----
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.expeditionStart);
        //-----FMOD Integration-----
        SceneManager.LoadSceneAsync(2);
    }

    public void CloseInventory()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        CraftingInventory.SetActive(false);
        CraftingButton.SetActive(false);
    }

    public void OpenInventory()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.papper);
        CraftingButton.SetActive(true);
        CraftingInventory.SetActive(true);
		Results.SetActive(true);
    }
	public void OpenRecipes()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.papper);
        Recipes.SetActive(true);
	}
	public void CloseRecipes()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Recipes.SetActive(false);
	}
	public void ResultsClose()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Results.SetActive(false);
	}


}
