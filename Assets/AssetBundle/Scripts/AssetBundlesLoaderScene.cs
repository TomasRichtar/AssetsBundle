using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MyAssetBundle
{
 
    public class AssetBundlesLoaderScene : MonoBehaviour
    {
        readonly private string _mainURL = "https://www.tastyair.cz/AssetBundleCity/";

        public event Action OnAssetsBundleFinishLoading;
        public event Action OnAssetsBundleStartLoading;

        [SerializeField] private Slider _slider;
        private AssetBundle myLoadedAssetBundle;
        private string[] scenePaths;
      
        private void Start()
        {
            StartCoroutine(DownloadAssetBundleFromServer());

        }
      
        private IEnumerator DownloadAssetBundleFromServer()
        {

            Debug.Log("Start loading");
            string url = _mainURL + "scene";
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                www.SendWebRequest();
                WaitForSeconds waitTime = new WaitForSeconds(0.2f);
                while (!www.isDone)
                {
                    _slider.value = www.downloadProgress;
                    yield return waitTime;
                }
                _slider.value = www.downloadProgress;
                // yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogWarning("ERROR on the get request at : " + url + " " + www.error);
                }
                else
                {
                    myLoadedAssetBundle = DownloadHandlerAssetBundle.GetContent(www);
                    /*gameObject = boundle.LoadAsset(boundle.GetAllAssetNames()[0]) as GameObject;
                    
                    boundle.Unload(false);*/
                    yield return new WaitForEndOfFrame();
                }
                www.Dispose();
            }
            
            Debug.Log("Finish loading");
            scenePaths = myLoadedAssetBundle.GetAllScenePaths();
            SceneManager.LoadScene(scenePaths[0], LoadSceneMode.Single);

        }

    }
}
