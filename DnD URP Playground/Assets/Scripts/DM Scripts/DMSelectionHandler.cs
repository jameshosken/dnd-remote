using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Script handles selection
public class DMSelectionHandler : MonoBehaviour
{
    public List<GameObject> allSelectedObjects = new List<GameObject>();
    GameObject selectedIndicator = null;

    Camera main;

    Material selectionMaterial;
    GenericMaterialsHandler materialsHandler;

    bool isDragging = false;
    Vector3 mouseDownPosition;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        selectionMaterial = Resources.Load("SelectionMaterial") as Material;
        //materialsHandler = FindObjectOfType<DMMaterialsHandler>();    //Todo implement materials change
    }

    private void Update()
    {
        if (isDragging)
        {
            Camera camera = Camera.main;

            //if (!Input.GetKey(KeyCode.LeftShift)) TryClearSelection();

            DMSelectable[] selectableObjects = FindObjectsOfType<DMSelectable>();
            for (int i = selectableObjects.Length-1; i >= 0; i--)
            {

                Bounds viewportBounds = GetViewportBounds(camera, mouseDownPosition, Input.mousePosition);
                if (viewportBounds.Contains(camera.WorldToViewportPoint(selectableObjects[i].transform.position)))
                {
                    GameObject attemptedSelection = selectableObjects[i].transform.root.gameObject;



                    if (!IsDuplicateSelection(attemptedSelection))
                    {

                        AddToSelection(attemptedSelection);
                    }
                    //allSelectedObjects.Add(selectables[i].gameObject);
                }
            }
        }
    }

    Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        Vector3 v1 = camera.ScreenToViewportPoint(screenPosition1);
        Vector3 v2 = camera.ScreenToViewportPoint(screenPosition2);
        Vector3 min = Vector3.Min(v1, v2);
        Vector3 max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }


    public void HandleSelectionClick()
    {
        Ray ray = main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        bool isSuccessfulSelection = false;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clicked = hit.collider.gameObject;

            if (clicked.GetComponentInParent<DMSelectable>())
            {
                isSuccessfulSelection = true;

                GameObject attemptedSelection = clicked.GetComponentInParent<DMSelectable>().gameObject;
                GameObject toRemove = null;
                Debug.Log("Attempting Selection: " + attemptedSelection.name);


                foreach (GameObject selected in allSelectedObjects)
                {
                    if (IsDuplicateSelection(attemptedSelection))
                    {
                        toRemove = attemptedSelection;
                        break;
                    }
                }

                if (toRemove) RemoveFromSelection(toRemove);
                else AddToSelection(attemptedSelection);
            }
        }

        if (!isSuccessfulSelection && Vector3.Distance(mouseDownPosition, Input.mousePosition) < 1f)
        {
            TryClearSelection();
        }
    }


    public void OnBeginDrag()
    {

        //Prevent clicks when over UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        mouseDownPosition = Input.mousePosition;
        isDragging = true;
        
    }


    public void OnEndDrag()
    {
        isDragging = false;
    }
    private bool IsDuplicateSelection(GameObject attemptedSelection)
    {
        foreach (GameObject selected in allSelectedObjects)
        {
            if (attemptedSelection.GetHashCode() == selected.GetHashCode())
            {
                return true;
            }
        }
        return false;
    }

    private void AddToSelection(GameObject toAdd)
    {
        Debug.Log("Adding to list: " + toAdd.name);
        ActivateSelectionShader(toAdd);
        allSelectedObjects.Add(toAdd);
    }

    public void TryClearSelection()
    {

        foreach (GameObject selected in allSelectedObjects)
        {

            TryDeactivateShader(selected);
        }
        allSelectedObjects.Clear();

    }

    void RemoveFromSelection(GameObject toRemove)
    {
        Debug.Log("Removing: " + toRemove.name);
        TryDeactivateShader(toRemove);
        allSelectedObjects.Remove(toRemove);
    }



    void TryDeactivateShader(GameObject sel)
    {
        if (sel != null)
        {
            DeactivateSelectionShader(sel);
        }
    }

    void ActivateSelectionShader(GameObject sel)
    {
        selectedIndicator = Instantiate(sel, sel.transform.position, sel.transform.rotation);
        DestroyImmediate(selectedIndicator.GetComponent<DMSelectable>());//Selection Outline is NOT selectable
        selectedIndicator.transform.parent = sel.transform;
        selectedIndicator.name = "Selection Indicator";

        Renderer[] renderers = selectedIndicator.GetComponentsInChildren<Renderer>();

        Collider[] colliders = selectedIndicator.GetComponentsInChildren<Collider>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = selectionMaterial;
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    void DeactivateSelectionShader(GameObject sel)
    {

        GameObject.DestroyImmediate(sel.transform.Find("Selection Indicator").gameObject);  //Don't wait until next frame; causes duplication issues.
    }

    public GameObject GetLatestSelection()
    {
        if(allSelectedObjects.Count > 0)
        {
            return allSelectedObjects[allSelectedObjects.Count - 1];
        }
        else
        {
            return null;
        }
    }
    public List<GameObject> GetAllSelectedObjects()
    {
        return allSelectedObjects;
    }
}
