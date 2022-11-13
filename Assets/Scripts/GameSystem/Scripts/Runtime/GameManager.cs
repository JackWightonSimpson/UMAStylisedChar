using System.Collections;
using System.Collections.Generic;
using GameSystem.Player;
using GameSystem.SaveLoad;
using GameSystem.Spawning;
using GameSystem.UI;
using GameSystem.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    //TODO:
    // Handle custom savedata
    // experiment with jsonUtility
    // better prefab spawning/management
    // player manager/spawner / positional scene transitions

    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private LoadingScreen loadingScreen;

        [SerializeField] private int mainMenuScene;
        [SerializeField] private int startScene;

        [SerializeField] private List<int> pauseMaps = new List<int>();
        [SerializeField] private InputActionAsset actions;
        
        public bool ActiveGame { get; private set; }

        [field: SerializeField] public bool Paused { get; private set; }
        public bool Loading { get; private set; }
        public bool HasFocus { get; private set; }

        private SaveManager saveManager;

        protected override void OnAwake()
        {
            ActiveGame = false;
            Loading = false;
            if (loadingScreen != null)
            {
                loadingScreen.gameObject.SetActive(false);
            }

            ActiveGame = SceneManager.GetActiveScene().buildIndex != mainMenuScene;
            saveManager = GetComponent<SaveManager>();
        }

        public bool IsGameScene(int sceneIndex)
        {
            return sceneIndex != mainMenuScene;
        }

        public void LoadGame(string savedata)
        {
            if (Loading)
            {
                Debug.LogError("Double load");
                return;
            }

            loadingScreen.gameObject.SetActive(true);
            Loading = true;
            StartCoroutine(DoLoadGame(savedata));
        }

        private IEnumerator DoLoadGame(string savedata)
        {
            PauseGame();
            if (ActiveGame)
            {
                loadingScreen.SetProgress("Unloading game", 0, "");
                yield return StartCoroutine(UnloadGame());
                ActiveGame = false;
            }

            var targetScene = startScene;
            if (savedata != null)
            {
                loadingScreen.SetProgress("Loading save data", 0, "");
                targetScene = saveManager.LoadGame(savedata);
            }

            var asyncLoad = SceneManager.LoadSceneAsync(targetScene);
            while (!asyncLoad.isDone)
            {
                loadingScreen.SetProgress("Loading scene", asyncLoad.progress, "");
                yield return null;
            }

            var loaded = SceneManager.GetSceneByBuildIndex(targetScene);
            SceneManager.SetActiveScene(loaded);
            loadingScreen.SetProgress("Setting up scene", 0, "");
            //init scene
            yield return StartCoroutine(LoadComplete(loaded));

            UnpauseGame();
            loadingScreen.gameObject.SetActive(false);
            loadingScreen.SetProgress("Loading...", 0, "");
            Loading = false;
        }

        private IEnumerator LoadComplete(Scene scene)
        {
            // yield return new WaitForFixedUpdate();
            PlayerManager.Instance.OnSceneLoad(scene);
            yield return null;
            if (!ActiveGame)
            {
                saveManager.OnSceneLoad(-1, LoadSceneMode.Single);
            }
            saveManager.OnSceneLoad(scene.buildIndex, LoadSceneMode.Single);
            PrefabManager.Instance.OnSceneLoad(scene);
            yield return null;
            ActiveGame = true;
        }

        private IEnumerator UnloadGame()
        {
            saveManager.ClearSaveData();
            yield return null;
        }

        public void LoadMainMenu()
        {
            if (Loading)
            {
                Debug.LogError("Double load");
                return;
            }

            loadingScreen.gameObject.SetActive(true);
            loadingScreen.SetProgress("Unloading game", 0, "");
            Loading = true;
            StartCoroutine(DoLoadMainMenu());
        }

        private IEnumerator DoLoadMainMenu()
        {
            PauseGame();
            if (ActiveGame)
            {
                yield return StartCoroutine(UnloadGame());
                ActiveGame = false;
            }

            loadingScreen.SetProgress("Loading menu", 0, "");
            var asyncLoad = SceneManager.LoadSceneAsync(mainMenuScene);
            while (!asyncLoad.isDone)
            {
                loadingScreen.SetProgress("Loading menu", asyncLoad.progress, "");
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(mainMenuScene));

            UnpauseGame();
            loadingScreen.gameObject.SetActive(false);
            loadingScreen.SetProgress("Loading...", 0, "");
            Loading = false;
        }

        public void ChangeScene(int sceneIndex)
        {
            if (Loading)
            {
                Debug.LogError("Double load");
                return;
            }

            loadingScreen.SetProgress("Loading scene", 0, "");
            Loading = true;
            StartCoroutine(DoChangeScene(sceneIndex));
        }

        private IEnumerator DoChangeScene(int sceneIndex)
        {
            PauseGame();
            Scene active = SceneManager.GetActiveScene();
            saveManager.OnSceneUnload(active);


            var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            while (!asyncLoad.isDone)
            {
                loadingScreen.SetProgress("Loading scene", asyncLoad.progress, "");
                yield return null;
            }

            active = SceneManager.GetSceneByBuildIndex(sceneIndex);
            SceneManager.SetActiveScene(active);
            loadingScreen.SetProgress("Setting up scene", 0, "");
            //init scene
            yield return StartCoroutine(LoadComplete(active));

            loadingScreen.gameObject.SetActive(false);
            loadingScreen.SetProgress("Loading...", 0, "");
            Loading = false;
            UnpauseGame();
        }
       
        
        //TODO: overlapping pause
        public void PauseGame()
        {
            Time.timeScale = 0;
            actions.actionMaps[0].Disable();
            Paused = true;
            SetCursorState();
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
            actions.actionMaps[0].Enable();
            Paused = false;
            SetCursorState();
        }

        private void SetCursorState()
        {
            if (!HasFocus)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if(Paused || !ActiveGame)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            HasFocus = hasFocus;
            SetCursorState();
        }
    }
}