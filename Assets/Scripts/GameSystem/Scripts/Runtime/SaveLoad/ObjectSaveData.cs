using System;
using System.Collections.Generic;
using System.Linq;
using GameSystem.Util;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameSystem.SaveLoad
{
    [Serializable]
    public class ObjectSaveData
    {
        public string id;
        public string prefabId;
        public string prefabPath;
        public bool singleton;
        
        public int sceneIndex;
        public string name;
        public string[] scenePath;
        public string parentId;
        
        public bool destroyed;
        // public SerializedVector3 position;
        // public SerializedVector3 scale;
        // public SerializedVector3 velocity;
        // public SerializeQuaternion rotation;
        // public SerializedVector3 angularVelocity;
        public Vector3 position;
        public Vector3 scale;
        public Vector3 velocity;
        public Quaternion rotation;
        public Vector3 angularVelocity;


        public SDictionary<string, SavedState> savedStates;

        public ObjectSaveData(){}
        public ObjectSaveData(Saveable go)
        {
            name = go.name;
            destroyed = false;
            prefabId = go.PrefabId;
            prefabPath = go.PrefabPath;
            id = go.ID;
            singleton = go.Singleton;
            UpdateFromGameObject(go);
        }
        
        public void UpdateFromGameObject(Saveable go)
        {
            var xform = go.transform;
            position = xform.position;
            rotation = xform.rotation;
            scale = xform.localScale;
            // scenePath = go.GetScenePath();
            if (go.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                velocity = rigidbody.velocity;
                angularVelocity = rigidbody.angularVelocity;
            }
            foreach (var component in go.GetComponents<ISaveSerializable>())
            {
                component.SetSaveState(this);
            }

            if (go.transform.parent != null)
            {
                var parent = go.transform.parent.GetComponent<Saveable>();
                if (parent != null)
                {
                    parentId = parent.ID;
                }
            }
        }
        
        public void LoadState(Saveable go)
        {
            if (destroyed )
            {
                Object.Destroy(go.gameObject);
                return;
            }
            var xform = go.transform;
            xform.position = position;
            xform.rotation = rotation;
            xform.localScale = scale;
            if (go.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.velocity = velocity;
                rigidbody.angularVelocity = angularVelocity;
            }
            if (go.TryGetComponent<CharacterController>(out var characterController))
            {
                characterController.enabled = false;
                xform.position = position;
                xform.rotation = rotation;
                characterController.enabled = true;
            }
            foreach (var component in go.GetComponents<ISaveSerializable>())
            {
                component.LoadSaveState(this);
            }
        }

        protected bool Equals(ObjectSaveData other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ObjectSaveData) obj);
        }

        public override int GetHashCode()
        {
            return (id != null ? id.GetHashCode() : 0);
        }

        public static bool operator ==(ObjectSaveData left, ObjectSaveData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObjectSaveData left, ObjectSaveData right)
        {
            return !Equals(left, right);
        }
    }
    
    
}