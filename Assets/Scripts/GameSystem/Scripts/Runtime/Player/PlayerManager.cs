using System.Collections.Generic;
using System.Linq;
using GameSystem.SaveLoad;
using GameSystem.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace GameSystem.Player
{
    public class PlayerManager : Singleton<PlayerManager>
    {

        [SerializeField] private SaveablePlayer playerPrefab;

        [SerializeField] private InputSystemUIInputModule uiInput;
        
        public SaveablePlayer activePlayer;
        private string targetSpawnId = "default";
        
        protected override void OnAwake()
        {
        }

        private void Start()
        {
            if (GameManager.Instance.ActiveGame && activePlayer == null)
            {
                SpawnPlayer();
            }
        }

        public void OnSceneLoad(Scene scene)
        {
            if (activePlayer == null)
            {
                SpawnPlayer();
            }
        }

        private void SpawnPlayer()
        {
            //TODO: spawn location//
            var spawn = FindObjectsOfType<PlayerSpawn>().FirstOrDefault(s => s.identifier.Equals(targetSpawnId));
            if (spawn == null)
            {
                spawn = FindObjectsOfType<PlayerSpawn>().FirstOrDefault(s => s.defaultSpawn);
            }

            if (spawn == null)
            {
                spawn = FindObjectsOfType<PlayerSpawn>().FirstOrDefault();
            }

            if (spawn != null)
            {
                activePlayer = Instantiate(playerPrefab);
                activePlayer.playerTransform.position = spawn.transform.position;
                activePlayer.playerTransform.rotation = spawn.transform.rotation;
                // activePlayer.playerTransform.GetComponent<PlayerInput>().uiInputModule = uiInput;
            }
        }
    }
}