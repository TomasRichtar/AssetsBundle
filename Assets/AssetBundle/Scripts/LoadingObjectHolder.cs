using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyAssetBundle
{
   
        public class LoadingObjectHolder : MonoBehaviour
    {
        public int testItemID;
        [Header("Object informations")]
        [Tooltip("Object Name == AssetBundle name on server")]
        public string ObjectName;
        [Tooltip("Low poly object")]
        public GameObject PreGameObject;
        public Transform Parent;

        [SerializeField] private bool _customAreaID;
        [Tooltip("Do you want to set custom Area or leave it to be automaticly?")]
        public List<int> MyAreaID = new List<int>();
        public int LOD;
        [Tooltip("Do you want to replace PreGameObject after AssetBungle load?")]
        public bool ReplaceObject = true;
        
      
        public void SetMyArea()//Set areas where this object is located
        {
            if (_customAreaID) return;
           
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5);
            MyAreaID.Clear();
            foreach (Collider item in hitColliders)
            {
                if (item.GetComponent<PlayerLocator>())
                {
                    MyAreaID.Add(item.GetComponent<PlayerLocator>().AreaID);
                }
            }
        }
        public void DestroyMyChild()
        {
            foreach (Transform item in gameObject.transform)
            {
                if (item !=null)
                {
                    DestroyImmediate(item.gameObject);

                }
            }
        }
        
       
    }
}
