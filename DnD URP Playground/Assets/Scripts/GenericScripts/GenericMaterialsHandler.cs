using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contributions by augutluhrs

public class GenericMaterialsHandler : MonoBehaviour
{

    [SerializeField] Dropdown materialDropdown; //hide for player

    List<Material> materialList = new List<Material>();

    Dictionary<string, Material> materialDictionary = new Dictionary<string, Material>();

    int currentMaterialIndex = 0;

    void Start()
    {

        LoadMaterials();
        PopulateMaterialDropdown();

        materialDropdown.onValueChanged.AddListener(delegate
        {
            OnMaterialDropdownChange();
        });
    }

    public Material GetMaterialByName(string name)
    {

        return materialDictionary[name];
    }

    private void LoadMaterials()
    {
        Material[] resourceMaterials = Resources.LoadAll<Material>("Object Materials");

        foreach (Material material in resourceMaterials)
        {
            materialList.Add(material);

            materialDictionary.Add(material.name, material);
        }
    }

    public Material GetCurrentMaterial()
    {
        return materialList[currentMaterialIndex];
    }
    
    public void OnMaterialDropdownChange()
    {
        currentMaterialIndex = materialDropdown.value;
    }

    public void CycleMaterial()
    {
        currentMaterialIndex = (currentMaterialIndex + 1) % materialList.Count;
        materialDropdown.value = currentMaterialIndex;
    }

    void PopulateMaterialDropdown()
    {
        
        materialDropdown.ClearOptions();   //Because we have to add options to dropdown in bulk
        List<string> matNames = new List<string>();
        foreach (Material mat in materialList)
        {
            matNames.Add(mat.name);
        }
        materialDropdown.AddOptions(matNames);


    }
}
