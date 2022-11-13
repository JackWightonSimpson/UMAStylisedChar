using System.Linq;
using GameSystem.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystem.Spawning
{
    public class PrefabManager : Singleton<PrefabManager>
    {
        
        [SerializeField] private PrefabDatabase prefabDatabase;

        private Transform sceneParent;
        
        protected override void OnAwake()
        {
            
        }

        public Transform DefaultParent()
        {
            return sceneParent;
        }

        public void OnSceneLoad(Scene scene)
        {
            sceneParent = GameObject.FindGameObjectsWithTag("DefaultSpawn").FirstOrDefault()?.transform;
        }
    }
}