using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyAssetBundle
{
    public class PlayerLocator : MonoBehaviour
    {
        [SerializeField] private AssetBundlesLoaderSimple _assetBundlesLoaderSimple;
        [SerializeField] private ObjectLoad _objectLoad;
        [SerializeField] private int _areaID;
        
        [SerializeField] private bool _isloaded = false;

        public int AreaID => _areaID;
        public bool IsLoaded => _isloaded;
        private void OnTriggerEnter(Collider other)
        {
            if(_isloaded) return;
            if (other.tag == "Player")
            {
               
                _assetBundlesLoaderSimple.DownloadAssetBundlePart(_areaID);
                

                _isloaded = true;
            }
        }
        
    }
}
