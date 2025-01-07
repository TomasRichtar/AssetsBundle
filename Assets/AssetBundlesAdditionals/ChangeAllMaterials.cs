using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ChangeAllMaterials : MonoBehaviour
{
    [SerializeField] private Material[] _materials;
    [SerializeField] private List<GameObject> _gameObjects;

    public void ChangeMaterials()
    {
        foreach (var item in _gameObjects)
        {
          
            foreach (Transform child in item.transform)
            {

               
                if (child.GetComponent<Renderer>())
                {
                    string materialName = child.GetComponent<Renderer>().material.name;

                    string materialCutName = materialName.Remove(materialName.Length - 11, 11);
                    Material childMaterial = Array.Find(_materials, x=>x.name == materialCutName);
                    Debug.Log(childMaterial);
                    /*
                    Shader shader = child.GetComponent<Renderer>().material.shader;
                    if (shader)
                    {

                        string shaderName = shader.name;
                        shader = Shader.Find(shaderName);
                        child.GetComponent<Renderer>().material.shader = shader;

                        
                    }*/
                    child.GetComponent<Renderer>().material = childMaterial;
                }
                child.gameObject.AddComponent<FadeMaterial>().IsFadingIn = true;
            }
        }
        
    }
    public Material GetMyMaterial(Material material)
    {
        string materialName = material.name;
        string materialCutName = materialName.Remove(materialName.Length - 11, 11);
        Material correctMaterial = Array.Find(_materials, x => x.name == materialCutName);
        return correctMaterial;
    }
    public void AddItem(GameObject myObject)
    {
        _gameObjects.Add(myObject);
    }
}
