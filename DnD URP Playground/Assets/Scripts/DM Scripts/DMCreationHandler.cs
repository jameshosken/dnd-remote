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

    [Space]
    [Header("Match this to dropdown")]
    public List<GameObject> placementObjects = new List<GameObject>();

    public int objectIdxToCreate = 0;

    DMInterfaceHandler interfaceHandler;

    // Start is called before the first frame update
    void Start()
    {
        interfaceHandler = FindObjectOfType<DMInterfaceHandler>();

        placementDropdown.onValueChanged.AddListener(delegate {
            UpdatePlacementIndex();
        });

        List<string> objNames = new List<string>();
        foreach (GameObject obj in placementObjects)
        {
            objNames.Add(obj.name);
        }
        placementDropdown.AddOptions(objNames);
    }

    void UpdatePlacementIndex()
    {
        objectIdxToCreate = placementDropdown.value;
    } 

    // Update is called once per frame
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

        placementObjects.Add(newObj);

        placementDropdown.ClearOptions();

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

            GameObject clone = Instantiate(placementObjects[objectIdxToCreate], placementIndicator.transform.position, Quaternion.identity) as GameObject;
            clone.name = placementObjects[objectIdxToCreate].name;
            clone.AddComponent<DMSelectable>();
        }


    }


}
