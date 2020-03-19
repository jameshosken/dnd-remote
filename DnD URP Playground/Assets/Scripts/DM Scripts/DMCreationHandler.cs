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
    [SerializeField] Transform placementIndicatorParent;
    public Dropdown placementDropdown;
    [SerializeField] DMSelectionHandler dmSelectionHandler;
    [SerializeField] Transform hiddenStorage;


    [Space]
    public List<GameObject> placementObjects = new List<GameObject>();
    List<GameObject> ghostIndicators = new List<GameObject>();  //Rename once secured

    public int objectIdxToCreate = 0;

    DMInterfaceHandler interfaceHandler;
    DMMaterialsHandler materialsHandler;

    bool wallMode = false;
    public float placementRotation = 0;

    //int wallClickCounter = 0;
    Vector3 wallStart = Vector3.zero;

    Vector3 wallEnd = Vector3.zero;


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

    public void ToggleWallMode(bool status)
    {
        wallMode = status;
    }
    void UpdatePlacementIndex()
    {
        objectIdxToCreate = placementDropdown.value;
    }

    public void RotatePlacement(int dir)
    {
        placementRotation += 90f * (float)dir;

        foreach (GameObject ghost in ghostIndicators)
        {
            ghost.transform.rotation = Quaternion.Euler(Vector3.up * placementRotation);
        }
    }

    void Update()
    {
        
        if (interfaceHandler.mode != DMInterfaceHandler.Mode.PLACE)
        {
            if (placementIndicatorParent.gameObject.activeSelf) { placementIndicatorParent.gameObject.SetActive(false); }
            return;
        }
        if (!placementIndicatorParent.gameObject.activeSelf) { placementIndicatorParent.gameObject.SetActive(true); }



        Vector3 mouseGridPos = interfaceHandler.GetMouseGridPosition();

        if (wallEnd != mouseGridPos)
        {
            if (!wallMode) wallStart = mouseGridPos;    //Only place one tile if not in wall mode
           
            wallEnd = mouseGridPos;
            List<Vector3> ghostPositions = CalculateLineFromLastClick();
            DestroyGhostIndicators();
            CreateGhostIndicators(ghostPositions);

        }

    }

    private void CreateGhostIndicators(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            GameObject indicator = Instantiate(placementIndicator, position, Quaternion.Euler(Vector3.up * placementRotation));
            indicator.transform.parent = placementIndicatorParent;
            ghostIndicators.Add(indicator);

        }
    }

    private void DestroyGhostIndicators()
    {
        foreach (GameObject ghost in ghostIndicators)
        {
            GameObject.Destroy(ghost);
        }
        ghostIndicators.Clear();
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

        //if (wallMode)
        //{

            List<Vector3> nodes = CalculateLineFromLastClick();

            foreach (Vector3 node in nodes)
            {
                CreateNewObject(node);
            }

            DestroyGhostIndicators();       //Clear Placement Indicators after updating
        //}
        //else
        //{
        //    CreateNewObject(placementIndicator.transform.position);
        //}


    }

    private List<Vector3> CalculateLineFromLastClick()
    {

        Vector3 from = wallStart;
        Vector3 to = wallEnd;
        //Vector3 to = interfaceHandler.GetMouseGridPosition(); //Todo: Fix bug?

        int cMax = 50;
        int xSign = (int)Mathf.Sign(to.x - from.x);
        int ySign = (int)Mathf.Sign(to.y - from.y);
        int zSign = (int)Mathf.Sign(to.z - from.z);

        List<Vector3> nodes = new List<Vector3>();

        for (int i = (int)from.x; i != (int)to.x; i += xSign)
        {
            if (i > cMax) break;
            if (i == (int)from.x) continue; //Todo fix this dumb hack
            nodes.Add(new Vector3(i, from.y, from.z));
        }
        nodes.Add(new Vector3(to.x, from.y, from.z));

        for (int i = (int)from.z; i != (int)to.z; i += zSign)
        {
            if (i > cMax) break;
            if (i == (int)from.z) continue;
            nodes.Add(new Vector3(to.x, from.y, i));
        }

        nodes.Add(new Vector3(to.x, from.y, to.z));
        for (int i = (int)from.y; i != (int)to.y; i += ySign)
        {
            if (i > cMax) break;
            if (i == (int)from.y) continue;
            nodes.Add(new Vector3(to.x, i, to.z));
        }

        nodes.Add(new Vector3(to.x, to.y, to.z));

        //wallStart = to;

        return nodes;
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
