using System;
using GameSystem.SaveLoad;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameSystem.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private InputActionReference triggerAction;
        [SerializeField] private InputActionAsset actions;


        [SerializeField] private GameObject menuParent;

        [SerializeField] private GameObject saveMenu;

        [SerializeField] private GameObject settingsMenu;

        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;


        private void Start()
        {
            actions.actionMaps[1].Enable();
            if (Application.isPlaying)
            {
                var gameScene = GameManager.Instance.IsGameScene(gameObject.scene.buildIndex);
                if (gameScene)
                {
                    menuParent.SetActive(false);
                }

                triggerAction.action.performed += ShowMenu;
                continueButton.gameObject.SetActive(!gameScene); //TODO save
                newGameButton.gameObject.SetActive(!gameScene);
                mainMenuButton.gameObject.SetActive(gameScene);
                
                settingsMenu.SetActive(false);
                saveMenu.SetActive(false);
                
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                triggerAction.action.performed -= ShowMenu;
            }
        }

        public void ShowMenu()
        {
            var gameScene = GameManager.Instance.IsGameScene(gameObject.scene.buildIndex);
            if (gameScene && menuParent.activeSelf)
            {
                GameManager.Instance.UnpauseGame();
                menuParent.SetActive(false);
            }
            else
            {
                GameManager.Instance.PauseGame();
                menuParent.SetActive(true);
            }
        }

        public void ShowMenu(InputAction.CallbackContext action)
        {
            ShowMenu();
        }

        public void LoadMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }

        public void StartGame()
        {
            GameManager.Instance.LoadGame(null);
        }

        public void ToggleSettings()
        {
            saveMenu.SetActive(false);
            settingsMenu.SetActive(!settingsMenu.activeSelf);
        }

        public void ToggleSaveLoad()
        {
            settingsMenu.SetActive(false);
            saveMenu.SetActive(!settingsMenu.activeSelf);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
        }
    }
}