using System;
using System.Collections.Generic;
using System.Linq;
using GameSystem.Util;
using UnityEngine;

namespace GameSystem.Spawning
{
    //TODO: better management of spawning objects - asset bundles + names? systems?
    [CreateAssetMenu(fileName = "prefab db", menuName = "save/prefab db", order = 0)]
    public class PrefabDatabase : ScriptableObject
    {
        [SerializeField]
        private SDictionary<string, string> prefabData = new SDictionary<string, string>();

        [SerializeField] 
        private int databaseSize;

        public GameObject LoadPrefab(string id)
        {
            return Resources.Load<GameObject>(prefabData[id]);
        }

        public void RegisterPrefab(string prefabId, string path)
        {
            prefabData[prefabId] = path;
        }

#if UNITY_EDITOR
        public void SetDatabase(Dictionary<string, string> data)
        {
            prefabData = data.ToSDictionary();
            databaseSize = data.Count;
        }
#endif
        

    }
    
    [Serializable]
    public class PrefabData
    {
        public string id;
        public string assetPath;
    }
}