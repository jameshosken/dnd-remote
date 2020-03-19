using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Contributions by augutluhrs

public class DMMaterialsHandler : MonoBehaviour
{

    [SerializeField] Dropdown materialDropdown;

    List<Material> materialList = new List<Material>();

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

    private void LoadMaterials()
    {
        Material[] resourceMaterials = Resources.LoadAll<Material>("Object Materials");

        foreach (Material material in resourceMaterials)
        {
            materialList.Add(material);
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
