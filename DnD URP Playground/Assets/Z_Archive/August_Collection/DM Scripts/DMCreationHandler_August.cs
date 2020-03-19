using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class DMCreationHandler_August : MonoBehaviour
{
    [SerializeField] GameObject placementIndicator;
    [SerializeField] Dropdown placementDropdown;
    [SerializeField] Dropdown materialDropdown;
    [SerializeField] DMSelectionHandler_August dmSelectionHandler;
    [SerializeField] Transform hiddenStorage;

    [Space]
    public List<GameObject> placementObjects = new List<GameObject>();
    public List<Material> materialList = new List<Material>();

    public int objectIdxToCreate = 0;
    public int materialIdxToAdd = 0;

    DMInterfaceHandler_August interfaceHandler;

    public float placementRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        interfaceHandler = FindObjectOfType<DMInterfaceHandler_August>();

        placementDropdown.onValueChanged.AddListener(delegate {
            UpdatePlacementIndex();
        });

        materialDropdown.onValueChanged.AddListener(delegate {
            UpdateMaterialIndex();
        });

        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");

        foreach (GameObject prefab in prefabs)
        {
            GameObject go = Instantiate(prefab);
            go.name = prefab.name;
            OnNewPlaceableObject(go);
        }

        SetUpMaterials();
    }

    void UpdatePlacementIndex()
    {
        objectIdxToCreate = placementDropdown.value;
    }
    
    void UpdateMaterialIndex()
    {
        materialIdxToAdd = materialDropdown.value;
    } 

    public void RotatePlacement(int dir)
    {
        placementRotation += 90f * (float)dir;
        placementIndicator.transform.rotation = Quaternion.Euler(Vector3.up * placementRotation);
    }

    void Update()
    {

        //Don't update if placing
        if (interfaceHandler.mode != DMInterfaceHandler_August.Mode.PLACE)
        {
            if (placementIndicator.activeSelf) { placementIndicator.SetActive(false); }
            return;
        }
        if (!placementIndicator.activeSelf) { placementIndicator.SetActive(true); }

        placementIndicator.transform.position = interfaceHandler.GetMouseGridPosition();
        
        HandleObjectPlacement();
    }


    void OnNewPlaceableObject(GameObject newObj)
    {
        newObj.transform.parent = hiddenStorage;

        placementObjects.Add(newObj);

        placementDropdown.ClearOptions();   //Because we have to add options to dropdown in bulk
        List<string> objNames = new List<string>();
        foreach (GameObject obj in placementObjects)
        {
            objNames.Add(obj.name);
        }
        placementDropdown.AddOptions(objNames);


    }

    void SetUpMaterials()
    {
        materialDropdown.ClearOptions();   //Because we have to add options to dropdown in bulk
        List<string> matNames = new List<string>();
        foreach (Material mat in materialList)
        {
            matNames.Add(mat.name);
        }
        materialDropdown.AddOptions(matNames);


    }


    private void HandleObjectPlacement()
    {

        //Prevent clicks when over UI:
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {

            GameObject clone = Instantiate(
                placementObjects[objectIdxToCreate], 
                placementIndicator.transform.position, 
                Quaternion.Euler(Vector3.up*placementRotation)
                ) as GameObject;
            clone.name = placementObjects[objectIdxToCreate].name;
            clone.GetComponentInChildren<MeshRenderer>().material = materialList[materialIdxToAdd];
            clone.AddComponent<DMSelectable>();
        }


    }


}
