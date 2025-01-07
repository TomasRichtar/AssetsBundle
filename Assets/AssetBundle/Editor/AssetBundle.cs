using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MyAssetBundle
{

    public class AssetBundle : MonoBehaviour
    {
        [MenuItem("Assets/Create Assets Bundles")]
        private static void BuildAllAssetBundles()
        {
            string assetBundleDirectionPath = Application.dataPath + "/../AssetsBundles";
            try
            {
                BuildPipeline.BuildAssetBundles(assetBundleDirectionPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                throw;
            }
        }
    }
}