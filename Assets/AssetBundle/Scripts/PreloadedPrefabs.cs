using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MyAssetBundle {
    [CustomEditor(typeof(PreloadedPrefabs))]
    public class PreloadedPrefabsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PreloadedPrefabs objectLoad = (PreloadedPrefabs)target;
            UnityEngine.Object block;
            var serializedObject = new SerializedObject(target);

            DrawDefaultInspector();

            
        }
    }
    [Serializable]
    public class AssetBundlesPrefab
    {
        public GameObject Preload;
        public GameObject LowPoly;
        public GameObject HighPoly;
    }
    public class PreloadedPrefabs : MonoBehaviour
    {
        private AssetBundlesWEB _assetBundlesWEB;

        public List<AssetBundlesPrefab> Prefabs = new List<AssetBundlesPrefab>();
       
    }
}
#endif