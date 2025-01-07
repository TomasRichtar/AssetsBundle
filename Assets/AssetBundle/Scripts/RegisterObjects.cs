using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
namespace MyAssetBundle
{
    [CustomEditor(typeof(ObjectLoad))]
    public class RegisterObjects : Editor
    {
        public override void OnInspectorGUI()
        {
            
            ObjectLoad objectLoad = (ObjectLoad)target;
            AssetBundlesLoaderSimple assetbundlesLoader = FindObjectOfType<AssetBundlesLoaderSimple>();

            objectLoad._debugMode = EditorGUILayout.Toggle("DebugMode", objectLoad._debugMode);

            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty("listPreloadedObjects");
            serializedObject.Update();
            EditorGUILayout.PropertyField(property, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Set Preloaded Objects"))
            {
                int maxLOD = 0;
                objectLoad.list = FindObjectsOfType<LoadingObjectHolder>();

                objectLoad.listPreloadedObjects = new PreloadedObject[objectLoad.list.Length];

                for (int i = 0; i < objectLoad.list.Length; i++)
                {
                    objectLoad.listPreloadedObjects[i].Name = objectLoad.list[i].ObjectName;
                    objectLoad.listPreloadedObjects[i].PreGameObject = objectLoad.list[i].PreGameObject;
                    objectLoad.listPreloadedObjects[i].Parent = objectLoad.list[i].Parent;
                    objectLoad.listPreloadedObjects[i].ReplaceObject = objectLoad.list[i].ReplaceObject;
                    objectLoad.listPreloadedObjects[i].LOD = objectLoad.list[i].LOD;
                    if (objectLoad.list[i].LOD > maxLOD)
                    {
                        maxLOD = objectLoad.list[i].LOD;
                    }
                    objectLoad.listPreloadedObjects[i].Parent.GetComponent<LoadingObjectHolder>().SetMyArea();
                    objectLoad.listPreloadedObjects[i].ObjectAreaID = objectLoad.listPreloadedObjects[i].Parent.GetComponent<LoadingObjectHolder>().MyAreaID;



                }

                assetbundlesLoader.AssetBundlesList.Clear();

                PlayerLocator[] areas = FindObjectsOfType<PlayerLocator>();
                int areaCount = areas.Length;
                
                for (int i = 0; i < areaCount; i++)
                {
                    PreloadedObject[] partZero = Array.FindAll(objectLoad.listPreloadedObjects, x => x.ObjectAreaID.Contains(i));
                    Dictionary<int, List<string>> LODDictionary = new Dictionary<int, List<string>>();

                    AssetBundles newAssetBundle = new AssetBundles();
                    if (newAssetBundle.AssetBundleLOD == null)
                        newAssetBundle.AssetBundleLOD = new List<LOD>();

                    foreach (var item in partZero)
                    {
                        List<string> newNames = new List<string>();
                        if (!LODDictionary.ContainsKey(item.LOD))
                        {
                            LODDictionary.Add(item.LOD, newNames);
                        }

                        if (LODDictionary[item.LOD].Contains(item.Name))
                            continue;

                        LODDictionary[item.LOD].Add(item.Name);
                    }
                    foreach (KeyValuePair<int, List<string>> ele in LODDictionary)
                    {
                        LOD newPart = new LOD();
                        newPart.AssetBundleName = ele.Value;
                        newAssetBundle.AssetBundleLOD.Add(newPart);
                    }
                    assetbundlesLoader.AssetBundlesList.Add(newAssetBundle);
                }
                

            }

        }
    }
}
#endif
