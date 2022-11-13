using System;
using System.Collections.Generic;
using System.Linq;
using GameSystem.Util;

namespace GameSystem.SaveLoad
{
    [Serializable]
    public class SceneState
    {
        public int sceneIndex;
        public SDictionary<string, ObjectSaveData> sceneData = new SDictionary<string, ObjectSaveData>();
        
        public List<ObjectSaveData> rawSceneData = new List<ObjectSaveData>();
        
        public void OnSave()
        {
        }

        public void OnLoad()
        {
        }

        protected bool Equals(SceneState other)
        {
            return sceneIndex == other.sceneIndex;
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

            return Equals((SceneState) obj);
        }

        public override int GetHashCode()
        {
            return sceneIndex;
        }

        public static bool operator ==(SceneState left, SceneState right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SceneState left, SceneState right)
        {
            return !Equals(left, right);
        }
    }
}