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
    DMMaterialsHandler materialsHandler;


    public float placementRotation = 0;

    //int wallClickCounter = 0;
    Vector3 wallStart = Vector3.zero;

    void Start()
    {
        interfaceHandler = FindObjectOfType<DMInterfaceHandler>();
        materialsHandler = FindObjectOfType<DMMaterialsHandler>();

        placementDropdown.onValueChanged.AddListener(delegate
        {
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
        //There's a better way to do this, involving a list of objects tat is active depepding on mode?

        //Don't update if not placing
        if (interfaceHandler.mode != DMInterfaceHandler.Mode.PLACE)
        {
            if (placementIndicator.activeSelf) { placementIndicator.SetActive(false); }
            return;
        }
        if (!placementIndicator.activeSelf) { placementIndicator.SetActive(true); }

        placementIndicator.transform.position = interfaceHandler.GetMouseGridPosition();

        //HandleObjectPlacement();
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

    public void HandleObjectPlacement()
    {


        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            CalculateLineFromLastClick();
        }
        else
        {
            CreateNewObject(placementIndicator.transform.position);
        }


    }

    private void CalculateLineFromLastClick()
    {


        Vector3 from = wallStart;
        Vector3 to = placementIndicator.transform.position;
        //Vector3 to = interfaceHandler.GetMouseGridPosition(); //Todo: Fix bug?

        int cMax = 50;
        int xSign = (int)Mathf.Sign(to.x - from.x);
        int ySign = (int)Mathf.Sign(to.y - from.y);
        int zSign = (int)Mathf.Sign(to.z - from.z);

        for (int i = (int)from.x; i != (int)to.x; i += xSign)
        {
            if (i > cMax) break;
            if (i == (int)from.x) continue; //Todo fix this dumb hack
            CreateNewObject(new Vector3(i, from.y, from.z));
        }
        CreateNewObject(new Vector3(to.x, from.y, from.z));

        for (int i = (int)from.z; i != (int)to.z; i += zSign)
        {
            if (i > cMax) break;
            if (i == (int)from.z) continue;
            CreateNewObject(new Vector3(to.x, from.y, i));
        }

        CreateNewObject(new Vector3(to.x, from.y, to.z));
        for (int i = (int)from.y; i != (int)to.y; i += ySign)
        {
            if (i > cMax) break;
            if (i == (int)from.y) continue;
            CreateNewObject(new Vector3(to.x, i, to.z));
        }

        CreateNewObject(new Vector3(to.x, to.y, to.z));

        wallStart = to;


    }

    void CreateNewObject(Vector3 pos)
    {
        //Todo: Not efficient, make better

        wallStart = pos;

        DMSelectable[] selectables = FindObjectsOfType<DMSelectable>();

        foreach (DMSelectable sel in selectables)
        {
            if(sel.gameObject.transform.position == pos && sel.gameObject.name == placementObjects[objectIdxToCreate].name)
            {
                print("Duplicate Creation");
                return;
            }
        }
        
        GameObject clone = Instantiate(
            placementObjects[objectIdxToCreate],
            pos,
            Quaternion.Euler(Vector3.up * placementRotation)
            ) as GameObject;
        clone.name = placementObjects[objectIdxToCreate].name;
        clone.AddComponent<DMSelectable>();

        //ADD MATERIAL TO NEW OBJECT

        foreach (Renderer renderer in clone.GetComponentsInChildren<Renderer>())
        {
            renderer.material = materialsHandler.GetCurrentMaterial();
        }

    }


}
