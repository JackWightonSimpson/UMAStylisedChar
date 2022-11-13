using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameSystem.Spawning;
using GameSystem.Util;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystem.SaveLoad
{
    public class SaveManager : Singleton<SaveManager>
    {
        public List<Saveable> activeObjects = new List<Saveable>();
        
        public SDictionary<int, SceneState> sceneStates = new SDictionary<int, SceneState>();
        
        private bool sceneUnloading = false;
        private bool gameLoading = false;
        
        [SerializeField]
        private PrefabDatabase prefabDatabase;
        
        protected override void OnAwake()
        {
            activeObjects.Clear();
            sceneStates.Clear();
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Saves")))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Saves"));
            }
        }

        public void RegisterSaveable(Saveable saveable)
        {
            if (!activeObjects.Contains(saveable))
            {
                activeObjects.Add(saveable);
            }
        }

        private void LoadObject(Saveable saveable)
        {
            var sceneState = GetSceneState(saveable, saveable.gameObject.scene);
            if (sceneState.sceneData.TryGetValue(saveable.ID, out var objectSaveData))
            {
                objectSaveData.LoadState(saveable);
            }
            else
            {
                sceneState.sceneData[saveable.ID] = saveable.GetData();
            }
        }

        private SceneState GetSceneState(Saveable saveable, Scene scene)
        {
            var sceneIndex = saveable.Singleton ? -1 : scene.buildIndex;
            SceneState sceneState;
            if (!sceneStates.TryGetValue(sceneIndex, out sceneState))
            {
                sceneState = new SceneState
                {
                    sceneIndex = scene.buildIndex,
                };
                sceneStates[sceneIndex] = sceneState;
            }
            return sceneState;
        }

        public void UnregisterSaveable(Saveable saveable)
        {
            var scene = SceneManager.GetActiveScene();

            activeObjects.Remove(saveable);
            if (!sceneUnloading)
            {
                Debug.Log(saveable.name);
                var sceneState = GetSceneState(saveable, saveable.gameObject.scene);
                ObjectSaveData objectSaveData;
                if (sceneState.sceneData.TryGetValue(saveable.ID, out objectSaveData))
                {
                    objectSaveData.UpdateFromGameObject(saveable);
                    objectSaveData.destroyed = true;
                }
            }
        }

        public void OnSceneLoad(int sceneIndex, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded");
            sceneUnloading = false;
            Dictionary<string, Transform> ids = new Dictionary<string, Transform>();
            foreach (var aSaveable in activeObjects)
            {
                if (!gameLoading && aSaveable.Singleton)
                {
                    continue;
                }
                LoadObject(aSaveable);
                ids.Add(aSaveable.ID, aSaveable.transform);
            }
            
            if (sceneStates.TryGetValue(sceneIndex, out var sceneState))
            {
                var data = sceneState.sceneData.Values.ToList();
                foreach (var saveData in data)
                {
                    if (!gameLoading && saveData.singleton)
                    {
                        continue;
                    }

                    if (ids.ContainsKey(saveData.id))
                    {
                        continue;
                    }

                    if (saveData.parentId != null && ids.TryGetValue(saveData.parentId, out var parent))
                    {
                        var instance = Instantiate(prefabDatabase.LoadPrefab(saveData.prefabId), parent);
                        var sav = instance.GetComponent<Saveable>();
                        sav.ID = saveData.id;
                        saveData.LoadState(instance.GetComponent<Saveable>());
                    }
                    else
                    {
                        Debug.LogError($"Could not load {saveData.id}, parent {saveData.parentId} does not exist");
                    }
                }
            }
            gameLoading = false;
        }
      
        
        
        public void OnSceneUnload(Scene scene)
        {
            sceneUnloading = true;
            if (gameLoading)
            {
                return;
            }
            SaveSceneState(scene);
        }
        
        private void SaveSceneState(Scene scene)
        {
            Debug.Log($"Start Save {activeObjects.Count}");
            foreach (var savable in activeObjects)
            {
                var sceneState = GetSceneState(savable, scene);

                if (sceneState.sceneData.TryGetValue(savable.ID, out var objectSaveData))
                {
                    objectSaveData.UpdateFromGameObject(savable);
                }
                else
                {
                    sceneState.sceneData[savable.ID] = savable.GetData();
                }
            }
            Debug.Log($"End Save {activeObjects.Count}");
        }
        
        public void SaveGame(string slotId)
        {
            Debug.Log($"Start Save for {slotId}");
            var activeScene = SceneManager.GetActiveScene();
            SaveSceneState(activeScene);
            var data = new SaveData
            {
                activeScene = activeScene.buildIndex,
                sceneStates = sceneStates.ToSDictionary(),
            };
            data.OnSave();

            var filePath = GetSaveFilePath(slotId);
            Debug.Log("Write data");
            // File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
            File.WriteAllText(filePath, JsonUtility.ToJson(data, true));
            Debug.Log("Write data done");
        }
        
        public int LoadGame(string slotId)
        {
            sceneUnloading = true;
            gameLoading = true;
            var filePath = GetSaveFilePath(slotId);
            // var state = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(filePath));
            var state = JsonUtility.FromJson<SaveData>(File.ReadAllText(filePath));
            if (state == null)
            {
                sceneStates = new SDictionary<int, SceneState>();
                return 0;
            }
            state.OnLoad();
            sceneStates = state.sceneStates;
            return state.activeScene;
        }

        public void ClearSaveData()
        {
            sceneStates.Clear();
            sceneUnloading = true;
        }

        public List<string> ListSaves()
        {
            var saves = Directory.EnumerateFiles(Path.Combine(Application.persistentDataPath, "Saves"))
                .Where(f => Path.HasExtension(".json")).Select(Path.GetFileNameWithoutExtension).ToList();
            return saves;
        }
        
        public void DeleteSave(string slotId)
        {
            File.Delete(GetSaveFilePath(slotId));
        }
        
        private string GetSaveFilePath(string slotId)
        {
            return Path.Combine(Application.persistentDataPath,"Saves", $"{slotId}.json");
        }
        
        public bool DoesSaveExist(string slotId)
        {
            return File.Exists(GetSaveFilePath(slotId));
        }
    }
}
