using MyAssetBundle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLoading : MonoBehaviour
{
    [SerializeField] private AssetBundlesLoaderSimple _assetBoundlesLoaderSimple;
    private void OnEnable()
    {
        _assetBoundlesLoaderSimple.OnAssetsBundleStartLoading += StartLoading;
        _assetBoundlesLoaderSimple.OnAssetsBundleFinishLoading += StopLoading;
    }
    private void OnDisable()
    {
        _assetBoundlesLoaderSimple.OnAssetsBundleStartLoading -= StartLoading;
        _assetBoundlesLoaderSimple.OnAssetsBundleFinishLoading -= StopLoading;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -1);
    }
    public void StartLoading()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }
    public void StopLoading()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
    }
}
