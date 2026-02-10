using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mainmenu : MonoBehaviour
{
	

	public GameObject Settings_menu;
	public GameObject MainMenu;
	public GameObject SoundPanel;
	public GameObject CreditsMenu;


	

	public void StartGame()
    {
        Debug.Log("This would start the game");
		 SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
	}

	public void ExitGame()
	{
		
        Application.Quit();
		Debug.Log("Exiting game");
	}

	public void Opensettings()
	{
		Debug.Log("I dont think we have a settings menu yet, add it when its done");
        Settings_menu.SetActive(true);
	}

	public void Closesettings()
	{
        
        Settings_menu.SetActive(false);
		PlayerPrefs.Save();
	}

	public void ReturnFromSound()
	{
        
        Settings_menu.SetActive(true);
		SoundPanel.SetActive(false);

	}

	public void OpenSounds()
	{
      
        SoundPanel.SetActive(true);
		Settings_menu.SetActive(false);

	}

	public void GoToSound()
	{
        
        Settings_menu.SetActive(false);
		SoundPanel.SetActive(true);
	
	}

	public void OpenCredits()
	{
		Debug.Log("we dont have any creadits yet, add some in the future");
		CreditsMenu.SetActive(true);

	}

	public void CloseCredits()
	{

		CreditsMenu.SetActive(false);

	}
}

