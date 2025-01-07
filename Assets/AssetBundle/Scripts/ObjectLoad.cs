using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace MyAssetBundle
{
    [Serializable]
    public struct PreloadedObject
    {
        public string Name;
        public GameObject PreGameObject;
        public Transform Parent;
        public List<int> ObjectAreaID;
        public int LOD;
        public bool ReplaceObject;
    }
    public struct UnloadedData
    {
        public string Name;
        public int LOD_ID;
    }
    public class ObjectLoad : MonoBehaviour
    {
        private PreloadedObject[] debugListPreloadedObjects;
        private AssetBundlesLoaderSimple _assetBundlessLoaderSimple;

        private Dictionary<string, GameObject> cachedGameObjects = new Dictionary<string, GameObject>();
        private Dictionary<string, List<int>> UnloadedGameObjects = new Dictionary<string, List<int>>();

        private List<string> cachedAssetBundleName = new List<string>();
        private PlayerLocator[] _playerLocator;
        private List<AssetBundles> assetList = new List<AssetBundles>();
        [SerializeField] private List<string> loadedAssets;

        public ChangeAllMaterials ChangeAllMaterial;//TMP
        public LoadingObjectHolder[] list;
        public PreloadedObject[] listPreloadedObjects;
        public List<int> _loadedParts = new List<int>();

        public Queue _unloadedQueue = new Queue();
        public bool _debugMode;
        private void Awake()
        {
            _assetBundlessLoaderSimple = GetComponent<AssetBundlesLoaderSimple>();
            _playerLocator = FindObjectsOfType<PlayerLocator>();
        }
        private void Start()
        {
            assetList = new List<AssetBundles>(_assetBundlessLoaderSimple.AssetBundlesList);
            if (!_debugMode) return;
            #region Debug
            LoadingObjectHolder[] objectLoad;
            objectLoad = FindObjectsOfType<LoadingObjectHolder>();
            //DELETED, ADDED
            if (objectLoad.Length > listPreloadedObjects.Length)
            {
                Debug.LogError("ERROR: You have created preloaded object. Please find ObjectLoad(Script) and PRESS \"Set Preloaded Objects\"");
            }
            else if (objectLoad.Length < listPreloadedObjects.Length)
            {
                Debug.LogError("ERROR: You have deleted your preloaded objects. Please find ObjectLoad(Script) and PRESS \"Set Preloaded Objects\"");
            }
            else if (objectLoad.Length == listPreloadedObjects.Length)
            {
                foreach (var item in listPreloadedObjects)
                {
                    if (item.PreGameObject == null)
                    {
                        Debug.LogError("ERROR: You have deleted your preloaded objects. Please find ObjectLoad(Script) and PRESS \"Set Preloaded Objects\"");
                    }
                }
            }
            //Wrong name
            List<string> validNamesList = new List<string>();
            foreach (var item in _assetBundlessLoaderSimple.AssetBundlesList)
            {
                foreach (var LODitem in item.AssetBundleLOD)
                {
                    foreach (var name in LODitem.AssetBundleName)
                    {
                        validNamesList.Add(name);
                    }
                }
            }
            foreach (var item in listPreloadedObjects)
            {
                if (!validNamesList.Contains(item.Name))
                {
                    Debug.LogError("ERROR: Your loading object have wrong name. " + item.Name + " Does not Exists.");
                }
            }
            //Not saved or didn't fixed all other errors
            LoadingObjectHolder[] debugList = FindObjectsOfType<LoadingObjectHolder>();

            debugListPreloadedObjects = new PreloadedObject[debugList.Length];

            LoadingObjectHolder[] list = FindObjectsOfType<LoadingObjectHolder>();
            for (int i = 0; i < list.Length; i++)
            {
                debugListPreloadedObjects[i].Name = list[i].ObjectName;
                debugListPreloadedObjects[i].PreGameObject = list[i].PreGameObject;
                debugListPreloadedObjects[i].Parent = list[i].Parent;
            }
            List<string> checkDebugList = new List<string>();
            List<string> CheckList = new List<string>();

            foreach (var item in debugListPreloadedObjects)
            {
                checkDebugList.Add(item.Name);
            }
            foreach (var item in listPreloadedObjects)
            {
                CheckList.Add(item.Name);
            }
            var set1 = new HashSet<string>(checkDebugList);
            var set2 = new HashSet<string>(CheckList);
            var areEqual = set1.SetEquals(set2);
            if (!areEqual)
            {
                Debug.LogError("ERROR: You have unsaved preloaded objects or you didn't fixed all the errors.  Please find ObjectLoad(Script) and PRESS \"Set Preloaded Objects\"");
            }
            #endregion
        }
        public void InstantiateGameObjectFromAssetBundle(GameObject gameObject, string assetBundleName, int partID)
        {

            cachedGameObjects.Add(assetBundleName, gameObject);
            InstantiateObjectsWithName(assetBundleName, partID);
        }
        public void ChecksOtherParts(GameObject gameObject, string assetBundleName)
        {
            List<int> items;
            if (!UnloadedGameObjects.TryGetValue(assetBundleName, out items))
            {
                return;
            }
            for (int i = 0; i < items.Count; i++)
            {
                InstantiateObjectsWithName(assetBundleName, items[i], true);
            }
            UnloadedGameObjects.Remove(assetBundleName);
        }
        public void InstantiateObjectsWithName(string assetBundleName, int partID, bool unloadedItems = false)
        {
            var SelectedObjects = listPreloadedObjects
                    .Where(l => l.Name == assetBundleName);

            GameObject SelectedGameobject;
            if (!cachedGameObjects.TryGetValue(assetBundleName, out SelectedGameobject))
            {
                List<int> items;
                if (UnloadedGameObjects.TryGetValue(assetBundleName, out items))
                {
                    items.Add(partID);
                }
                else
                {
                    UnloadedGameObjects.Add(assetBundleName, new List<int> { partID });
                }
                return;
            }

            if (!unloadedItems)
            {
                ChecksOtherParts(SelectedGameobject, assetBundleName);
            }

            foreach (var item in SelectedObjects)
            {
                if (!item.ObjectAreaID.Contains(partID))
                {
                    continue;
                }
                if (item.Parent.childCount >= 2)//TODO do budoucna mùže být problém, takže najít jiný zpùsob aby se nevytváøel objekt pro každou lakaci ve které je
                {
                    continue;
                }
                InstantiateObject(SelectedGameobject, item);
            }
        }

        public void InstantiateObject(GameObject gameObject, PreloadedObject data)
        {
            GameObject instanceGameObject = Instantiate(gameObject, data.Parent.transform.position,
                data.Parent.transform.rotation, data.Parent.transform);
#if UNITY_EDITOR
            ShaderCheck(instanceGameObject.GetComponent<Renderer>());
            ShaderCheckChilds(instanceGameObject.transform);
#endif
            if (data.ReplaceObject && data.PreGameObject)
            {
                data.PreGameObject.SetActive(false);
            }
        }

        private void ShaderCheck(Renderer r)
        {
            if (!r)
                return;

            foreach (var mat in r.materials)
            {
                Shader shader = mat.shader;
                if (shader)
                {
                    string shaderName = shader.name;
                    shader = Shader.Find(shaderName);
                    mat.shader = shader;
                }
            }
        }
        private void ShaderCheckChilds(Transform parent)
        {
            foreach (Transform child in parent)
            {
                ShaderCheck(child.GetComponent<Renderer>());
                if (child.childCount > 0)
                {
                    ShaderCheckChilds(child);
                }
            }
        }
    }
}
