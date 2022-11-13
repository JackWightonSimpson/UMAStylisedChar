using System.Collections.Generic;
using GameSystem.SaveLoad;
using GameSystem.Spawning;
using UnityEditor;
using UnityEngine;

namespace GameSystem
{
    [CustomEditor(typeof(PrefabDatabase),true)]
    public class PrefabDatabaseEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var database = (PrefabDatabase)target;
            DrawDefaultInspector();

            if (GUILayout.Button("Build Db"))
            {
                var data = new Dictionary<string, string>();
                var assets = AssetDatabase.FindAssets($"t:Prefab");
                foreach (var assetGuid in assets)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (asset.TryGetComponent<Saveable>(out var saveable))
                    {
                        Debug.Log(asset.name);
                        assetPath = assetPath.Replace("Assets/Resources/", "");
                        assetPath = assetPath.Replace(".prefab", "");
                        data[saveable.PrefabId] = assetPath;
                    }
                }
                database.SetDatabase(data);
                EditorUtility.SetDirty(database);
            }
        }
    }
}