using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System;

namespace MyAssetBundle
{
    public class test
    {
        public static List<int> indexList = new List<int>();
        public static List<LoadingObjectHolder> objectHolderList = new List<LoadingObjectHolder>();
        public static int selectedIndex;

    }
    public enum TypeOfObject
    {
        preloaded,
        lowPoly,
        highPoly
    }
    [CustomEditor(typeof(LoadingObjectHolder))]
    public class LoadingObjectHelperEditor : Editor
    {
        public int selected;
        AssetBundlesWEB assetBundlesWEB;
        LoadingObjectHolder objectLoad;
        LoadingObjectHolder[] partZero;

        private void OnValidate()
        {
            test.indexList.Add(selected);
            Debug.Log(test.indexList.Count);
        }
        public override void OnInspectorGUI()
        {
            objectLoad = (LoadingObjectHolder)target;
            
            assetBundlesWEB = FindObjectOfType<AssetBundlesWEB>();
            
            
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty("ObjectName");
            var property1 = serializedObject.FindProperty("PreGameObject");
            var property2 = serializedObject.FindProperty("Parent");
            var property3 = serializedObject.FindProperty("_customAreaID");
            var property4 = serializedObject.FindProperty("MyAreaID");
            var property5 = serializedObject.FindProperty("LOD");
            var property6 = serializedObject.FindProperty("ReplaceObject");
            var property7 = serializedObject.FindProperty("testItemID");
            if (!test.objectHolderList.Contains(objectLoad))
            {
                test.objectHolderList.Add(objectLoad);
                test.indexList.Add(objectLoad.testItemID);
            }
             partZero = Array.FindAll(test.objectHolderList.ToArray(), x => x == objectLoad);

            if (assetBundlesWEB != null)
                partZero[0].testItemID = EditorGUILayout.Popup(
                "SelectedObject", partZero[0].testItemID, assetBundlesWEB.Names.ToArray(), EditorStyles.popup);


            serializedObject.Update();

            if (GUILayout.Button("Create preloaded object"))
            {
                CreateObject(TypeOfObject.preloaded);
            }
            if (GUILayout.Button("Create lowpoly object"))
            {
                CreateObject(TypeOfObject.lowPoly);
            }
            if (GUILayout.Button("Create highpoly object"))
            {
                CreateObject(TypeOfObject.highPoly);
            }
            if (GUILayout.Button("Create all preloaded object"))
            {
                CreateAll(TypeOfObject.preloaded);
            }
            if (GUILayout.Button("Create all lowpoly object"))
            {
                CreateAll(TypeOfObject.lowPoly);
            }
            EditorGUILayout.PropertyField(property);
            EditorGUILayout.PropertyField(property1);
            EditorGUILayout.PropertyField(property2);
            EditorGUILayout.PropertyField(property3);
            EditorGUILayout.PropertyField(property4);
            EditorGUILayout.PropertyField(property5);
            EditorGUILayout.PropertyField(property6);
            EditorGUILayout.PropertyField(property7);


            serializedObject.ApplyModifiedProperties();


        }
        private void CreateAll(TypeOfObject type)
        {
            PreloadedPrefabs preloadedPrefab;
            preloadedPrefab = FindObjectOfType<PreloadedPrefabs>();
            
            
            LoadingObjectHolder[] loadingObjectHolder;
            loadingObjectHolder = FindObjectsOfType<LoadingObjectHolder>();

            if (loadingObjectHolder != null)
                foreach (var item in loadingObjectHolder)
            {
                item.Parent = item.transform;
                item.DestroyMyChild();
                GameObject preloadedObject = null;
                if (preloadedPrefab != null)
                    switch (type)
                {
                    case TypeOfObject.preloaded:
                        preloadedObject =
                 Instantiate(preloadedPrefab.Prefabs[item.testItemID].Preload, item.gameObject.transform);
                        break;
                    case TypeOfObject.lowPoly:
                        preloadedObject =
                  Instantiate(preloadedPrefab.Prefabs[item.testItemID].LowPoly, item.gameObject.transform);
                        break;
                    case TypeOfObject.highPoly:
                        preloadedObject =
                  Instantiate(preloadedPrefab.Prefabs[item.testItemID].HighPoly, item.gameObject.transform);
                        break;
                    default:
                        preloadedObject =
                  Instantiate(preloadedPrefab.Prefabs[item.testItemID].Preload, item.gameObject.transform);
                        break;
                }
                if (preloadedPrefab != null)
                    item.PreGameObject = preloadedObject;
                item.name = "Holder_" + assetBundlesWEB.Names[item.testItemID];
                item.ObjectName = assetBundlesWEB.Names[item.testItemID];
            }
        }
        private void CreateObject(TypeOfObject type)
        {
            PreloadedPrefabs preloadedPrefab;
            preloadedPrefab = FindObjectOfType<PreloadedPrefabs>();
            objectLoad.Parent = objectLoad.transform;
            objectLoad.DestroyMyChild();

            GameObject preloadedObject=null;
            if (preloadedPrefab != null)
                switch (type)
            {
                case TypeOfObject.preloaded:
                     preloadedObject =
              Instantiate(preloadedPrefab.Prefabs[partZero[0].testItemID].Preload, objectLoad.gameObject.transform);
                    break;
                case TypeOfObject.lowPoly:
                    preloadedObject =
              Instantiate(preloadedPrefab.Prefabs[partZero[0].testItemID].LowPoly, objectLoad.gameObject.transform);
                    break;
                case TypeOfObject.highPoly:
                    preloadedObject =
              Instantiate(preloadedPrefab.Prefabs[partZero[0].testItemID].HighPoly, objectLoad.gameObject.transform);
                    break;
                default:
                    preloadedObject =
              Instantiate(preloadedPrefab.Prefabs[partZero[0].testItemID].Preload, objectLoad.gameObject.transform);
                    break;
                }
            if (preloadedPrefab != null)
                objectLoad.PreGameObject = preloadedObject;
            objectLoad.name = "Holder_" + assetBundlesWEB.Names[partZero[0].testItemID];
            objectLoad.ObjectName = assetBundlesWEB.Names[partZero[0].testItemID];

        }

    }

}
#endif