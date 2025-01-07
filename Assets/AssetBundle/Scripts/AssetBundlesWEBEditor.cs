
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;


namespace MyAssetBundle
{
    [CustomEditor(typeof(AssetBundlesWEB))]
    public class WebHelper : Editor

    {
        public override void OnInspectorGUI()
        {

            AssetBundlesWEB objectLoad = (AssetBundlesWEB)target;

           
            var serializedObject = new SerializedObject(target);
            var property1 = serializedObject.FindProperty("Names");
            var property2 = serializedObject.FindProperty("phpURL");
            var property3 = serializedObject.FindProperty("ftpdir");
            serializedObject.Update();
            EditorGUILayout.PropertyField(property1, true);
            EditorGUILayout.PropertyField(property2, true);
            EditorGUILayout.PropertyField(property3, true);
            serializedObject.ApplyModifiedProperties();


            if (GUILayout.Button("Set Preloaded Objects"))
            {
               _ = objectLoad.LoadUrls(property2.stringValue, property3.stringValue);
            }
        }
       
    }
}
#endif