using _Project._Scripts.Sound_and_Music;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mainmenu : MonoBehaviour
{
	

	public GameObject Settings_menu;
	public GameObject MainMenu;
	public GameObject SoundPanel;
	public GameObject CreditsMenu;

	public void Start()
	{
		//-----FMOD Integration-----
		AudioManager.Instance.PlayMusic(FMODEvents.instance.mainMenu);
    }


    public void StartGame()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Debug.Log("This would start the game");
        AudioManager.Instance.StopMusic();
         SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
	}

	public void ExitGame()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Application.Quit();
		Debug.Log("Exiting game");
	}

	public void Opensettings()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
		Debug.Log("I dont think we have a settings menu yet, add it when its done");
        Settings_menu.SetActive(true);
	}

	public void Closesettings()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Settings_menu.SetActive(false);
		PlayerPrefs.Save();
	}

	public void ReturnFromSound()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Settings_menu.SetActive(true);
		SoundPanel.SetActive(false);

	}

	public void OpenSounds()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        SoundPanel.SetActive(true);
		Settings_menu.SetActive(false);

	}

	public void GoToSound()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Settings_menu.SetActive(false);
		SoundPanel.SetActive(true);
	
	}

	public void OpenCredits()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        Debug.Log("we dont have any creadits yet, add some in the future");
		CreditsMenu.SetActive(true);

	}

	public void CloseCredits()
	{
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.buttonPress);
        CreditsMenu.SetActive(false);

	}

	
}

