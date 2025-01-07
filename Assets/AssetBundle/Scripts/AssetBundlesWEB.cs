using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MyAssetBundle
{
    [Serializable]
    public struct MyAssetBundlesStruct
    {
        public string Name;
        public GameObject Prefab;
    }
    public class AssetBundlesWEB : MonoBehaviour
    {
        public string phpURL;
        public string ftpdir;
        public List<string> Names = new List<string>();
        public List<MyAssetBundlesStruct> AssetBundlesSetUp = new List<MyAssetBundlesStruct>();
        public List<MyAssetBundlesStruct> AssetBundlesSetUpSaved = new List<MyAssetBundlesStruct>();
        private MyAssetBundlesStruct mystruct;
     
        public async Task LoadUrls(string url,string ftpdir)
        {
            Names.Clear();
            AssetBundlesSetUp.Clear();
            WWWForm form = new WWWForm();
            form.AddField("ftpdir", ftpdir);
            UnityWebRequest www = UnityWebRequest.Post(url + "GetAllFileNames.php", form);
            await www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                List<string> urls = result.Split('\n').ToList();
                urls.RemoveRange(0, 4);
                urls.RemoveAt(urls.Count - 1);
                foreach (var item in urls)
                {
                    string name = item.Remove(0, ftpdir.Length + 1);
                    if (!Names.Contains(name))
                    {
                        Names.Add(name);
                    }

                    mystruct.Name = name;
                    mystruct.Prefab = null;

                    AssetBundlesSetUp.Add(mystruct);
                }
            }
            www.Dispose();
        }
      
    }
}
