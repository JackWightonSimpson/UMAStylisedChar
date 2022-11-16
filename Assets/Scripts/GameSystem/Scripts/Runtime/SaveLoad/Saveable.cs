using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace GameSystem.SaveLoad
{
    [ExecuteAlways]
    public class Saveable : MonoBehaviour
    {
        [field: SerializeField]
        [field: ReadOnly]
        public string[] ScenePath { get; private set; }

        [field: SerializeField] public string ID { get; set; }

        [field: SerializeField] public bool Singleton { get; private set; }

        [field: SerializeField] public string PrefabId { get; private set; }
        [field: SerializeField] public string PrefabPath { get; private set; }

        
        // [SerializeField] 
        private int instanceID = 0;
        
        private void Awake()
        {
            Debug.Log($"{instanceID} - {GetInstanceID()}");
            if (Application.isPlaying)
            {
                if (ScenePath == null)
                {
                    GetScenePath();
                }
                //Handle runtime spawns
                if (ID == PrefabId && !Singleton)
                {
                    ID = Guid.NewGuid().ToString();
                }
            }
            else
            {
                Debug.Log($"{instanceID} - {GetInstanceID()}");
                if (instanceID == 0)
                {
                    instanceID = GetInstanceID();
                }
                else if (instanceID != GetInstanceID()) // && GetInstanceID() < 0)
                {
                    instanceID = GetInstanceID();
                    ID = Guid.NewGuid().ToString();
                }
            }
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                SaveManager.Instance.RegisterSaveable(this);
            }
        }

        public string[] GetScenePath()
        {
            var pathElements = new List<string>();
            var c = transform;
            while (c != null)
            {
                pathElements.Add(c.name);
                c = transform.parent;
            }

            ScenePath = pathElements.ToArray().Reverse().ToArray();
            return ScenePath;
        }

        private void Reset()
        {
            Debug.Log("reset: " + name);
            ID = Guid.NewGuid().ToString();
        }

        private void OnValidate()
        {
            // Debug.Log($"{instanceID} - {GetInstanceID()}");
            if (ID == null)
            {
                ID = Guid.NewGuid().ToString();
            }
#if UNITY_EDITOR
            //Check and assign prefab id
            if (PrefabId == null && (gameObject.scene.name == null || gameObject.scene.name == gameObject.name) &&
                transform.parent == null)
            {
                PrefabId = ID;
                //TODO: add to prefab db
            }
            // if (PrefabUtility.IsPartOfPrefabInstance(this))
            // {
            //     if (ID == PrefabId && !Singleton)
            //     {
            //         ID = Guid.NewGuid().ToString();
            //     }
            // }
#endif
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                SaveManager.Instance.UnregisterSaveable(this);
            }
        }

        public ObjectSaveData GetData()
        {
            return new ObjectSaveData(this);
        }
    }
}