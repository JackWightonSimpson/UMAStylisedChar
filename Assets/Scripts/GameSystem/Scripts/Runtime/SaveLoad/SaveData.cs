using System;
using System.Collections.Generic;
using System.Linq;
using GameSystem.Util;
using UnityEngine;

namespace GameSystem.SaveLoad
{
    [Serializable]
    public class SaveData
    {
        public SDictionary<int, SceneState> sceneStates = new SDictionary<int, SceneState>();

        public int activeScene = 0;

        public void OnSave()
        {
        }

        public void OnLoad()
        {
        }
    }
}