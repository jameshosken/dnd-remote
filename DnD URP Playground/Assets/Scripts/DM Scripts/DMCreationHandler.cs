using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class DMCreationHandler : MonoBehaviour
{
    [SerializeField] GameObject placementIndicator;
    [SerializeField] Dropdown placementDropdown;
    [SerializeField] DMSelectionHandler dmSelectionHandler;
    [SerializeField] Transform hiddenStorage;

    [Space]
    public List<GameObject> placementObjects = new List<GameObject>();

    public int objectIdxToCreate = 0;

    DMInterfaceHandler interfaceHandler;

    public float placementRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        interfaceHandler = FindObjectOfType<DMInterfaceHandler>();

        placementDropdown.onValueChanged.AddListener(delegate {
            UpdatePlacementIndex();
        });

        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");

        foreach (GameObject prefab in prefabs)
        {
            GameObject go = Instantiate(prefab);
            go.name = prefab.name;
            OnNewPlaceableObject(go);
        }
    }

    void UpdatePlacementIndex()
    {
        objectIdxToCreate = placementDropdown.value;
    } 

    public void RotatePlacement(int dir)
    {
        placementRotation += 90f * (float)dir;
        placementIndicator.transform.rotation = Quaternion.Euler(Vector3.up * placementRotation);
    }

    void Update()
    {

        //Don't update if placing
        if (interfaceHandler.mode != DMInterfaceHandler.Mode.PLACE)
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
            clone.AddComponent<DMSelectable>();
        }


    }


}
