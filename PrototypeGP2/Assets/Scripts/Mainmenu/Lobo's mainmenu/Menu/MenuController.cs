using UnityEngine;

namespace UI.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Settings")]
        [SerializeField] private bool startWithMainMenuOpen = true;

        private void Start()
        {
            if (startWithMainMenuOpen)
            {
                OpenMainMenu();
            }
        }

        public void OpenMainMenu()
        {
            Debug.Log("main menu opened??");
            SetActivePanel(mainMenuPanel);
        }

        public void OpenCredits()
        {
            SetActivePanel(creditsPanel);
        }

        public void OpenPauseMenu()
        {
            SetActivePanel(pausePanel);
        }

        public void PlayOrResume()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            
            Debug.Log("game start / resumed");
        }

        public void QuitGame()
        {
            Debug.Log("put quit game here");
        }

        private void SetActivePanel(GameObject activePanel)
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(mainMenuPanel == activePanel);
            if (creditsPanel != null) creditsPanel.SetActive(creditsPanel == activePanel);
            if (pausePanel != null) pausePanel.SetActive(pausePanel == activePanel);
        }
    }
}
