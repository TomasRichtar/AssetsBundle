
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace MyAssetBundle
{
    /* [Serializable]
     public struct PartsToDownload
     {
         public string[] AssetbundleNames;
     }*/
    [Serializable]
    public struct LOD
    {
        public List<string> AssetBundleName;
    }
    [Serializable]
    public struct AssetBundles
    {
        public List<LOD> AssetBundleLOD;
    }
    public struct DownloadData
    {
        public int Part_ID;
        public int LOD_ID;
        public int AssetBundle_ID;
    }
    public class AssetBundlesLoaderSimple : MonoBehaviour
    {
        [Header("AssetBundles URL")]
        [SerializeField] private string _mainURL = "https://www.tastyair.cz/AssetBundleTractor/";
        public event Action OnStartDownload;
        public event Action OnAssetsBundleFinishLoading;
        public event Action OnAssetsBundleStartLoading;

        [SerializeField] private List<AssetBundles> _assetBundles;
        [SerializeField] private List<int> loadedPartID;
        [SerializeField] private List<string> loadedAssets;

        private ObjectLoad _objectLoad;
        private int _tryLoadCount = 0;
        public Queue _queue = new Queue();
        private bool _isDownloading;
        private List<DownloadData> _downloadData = new List<DownloadData>();
        public List<AssetBundles> AssetBundlesList { get => _assetBundles; set => _assetBundles = value; }


        private void Awake()
        {
            _objectLoad = GetComponent<ObjectLoad>();
        }

        public void DownloadQueue(int PartID, int AssetBundleID, int LOD_ID)
        {
            //Adding to Queue
            _queue.Enqueue(new DownloadData()
            {
                Part_ID = PartID,
                LOD_ID = LOD_ID,
                AssetBundle_ID = AssetBundleID
            });
            //Download from queue
            CheckQueue();
        }
        public void CheckQueue()
        {
            if (!_isDownloading)//Waiting for the last download
            {
                if (_queue.Count != 0)//Checks if there is something to download
                {
                    var x = _queue.Dequeue();
                    DownloadData data = (DownloadData)x;
                    //Starts downloading
                    StartCoroutine(DownloadAssetBundleFromServer(data.Part_ID, data.AssetBundle_ID, data.LOD_ID));
                }
                else
                {
                    Debug.Log("Empty");
                }
            }
        }
        public void DownloadAssetBundlePart(int partID)
        {
            if (loadedPartID.Contains(partID))//If part is already loaded or downloading
            {
                Debug.Log("return");
                return;
            }
            loadedPartID.Add(partID);

            for (int lod = 0; lod < _assetBundles[partID].AssetBundleLOD.Count; lod++)//Add to queue every object in every lod in PartID
            {
                for (int i = 0; i < _assetBundles[partID].AssetBundleLOD[lod].AssetBundleName.Count; i++)
                {
                    //If AssetBundle is already loaded or downloading
                    if (loadedAssets.Contains(_assetBundles[partID].AssetBundleLOD[lod].AssetBundleName[i]))
                    {
                        _objectLoad.InstantiateObjectsWithName(_assetBundles[partID].AssetBundleLOD[lod].AssetBundleName[i], partID);
                        continue;
                    }
                    OnAssetsBundleStartLoading?.Invoke();
                    loadedAssets.Add(_assetBundles[partID].AssetBundleLOD[lod].AssetBundleName[i]);//Add to list
                    DownloadQueue(partID, i, lod);//Added to queue
                }
            }
        }
        private IEnumerator DownloadAssetBundleFromServer(int PartID, int AssetBundleID, int LOD_ID)
        {
            _isDownloading = true; //Register downloading
            GameObject gameObject = null;
            //URL
            string url = _mainURL + _assetBundles[PartID].AssetBundleLOD[LOD_ID].AssetBundleName[AssetBundleID];
            float startLoadingTime = Time.realtimeSinceStartup;
           // Debug.Log("Start loading: " + _assetBundles[PartID].AssetBundleLOD[LOD_ID].AssetBundleName[AssetBundleID]);

            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogWarning("ERROR on the get request at : " + url + " " + www.error);
                    if (_tryLoadCount < 20)//reload
                    {
                        _tryLoadCount++;
                        yield return DownloadAssetBundleFromServer(PartID, AssetBundleID, LOD_ID);
                        
                    }
                }
                else
                {
                    //Bundle to gameObject
                    AssetBundle boundle = DownloadHandlerAssetBundle.GetContent(www);
                    gameObject = boundle.LoadAsset(boundle.GetAllAssetNames()[0]) as GameObject;

                    boundle.Unload(false);
                    yield return new WaitForEndOfFrame();
                }
                www.Dispose();
            }
            float endLoadingTime = Time.realtimeSinceStartup;
            /*  Debug.Log("Finish loading: " + _assetBundles[PartID].AssetBundleLOD[LOD_ID].AssetBundleName[AssetBundleID]
              + " in time: " + (endLoadingTime - startLoadingTime));*/
            //Call the Instantiate
            if (gameObject != null)
            {
                _objectLoad.InstantiateGameObjectFromAssetBundle(
                    gameObject, _assetBundles[PartID].AssetBundleLOD[LOD_ID].AssetBundleName[AssetBundleID], PartID);
            }
            else//Reload
            {
                Debug.LogWarning("Your AssetBoundle is NULL");
                if (_tryLoadCount < 20)
                {
                    _tryLoadCount++;
                    yield return DownloadAssetBundleFromServer(PartID, AssetBundleID, LOD_ID);
                }
            }
            _isDownloading = false;//Unregister downloading
            CheckQueue();//go back to queue and try download next object
        }

        public void DownloadAllAssetBundlePart()
        {
            for (int i = 0; i < _assetBundles.Count; i++)
            {
                DownloadAssetBundlePart(i);
            }
        }
    }
}
