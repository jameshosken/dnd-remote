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


    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        selectionMaterial = Resources.Load("SelectionMaterial") as Material;
        //materialsHandler = FindObjectOfType<DMMaterialsHandler>();    //Todo implement materials change
    }


    public void HandleSelection()
    {
        Ray ray = main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        bool isSuccessfulSelection = false; 

        //Prevent clicks when over UI:
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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

        if (!isSuccessfulSelection)
        {
            TryClearSelection();
        }
    }

    private bool IsDuplicateSelection(GameObject attemptedSelection)
    {
        foreach (GameObject selected in allSelectedObjects)
        {
            if (attemptedSelection.GetHashCode() == selected.GetHashCode())
            {
                Debug.Log("Selection already in list, flag to remove: " + attemptedSelection);
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

        Debug.Log("Cleared Selection");
    }

    void RemoveFromSelection(GameObject toRemove)
    {
        Debug.Log("Removing: " + toRemove.name);
        TryDeactivateShader(toRemove);
        allSelectedObjects.Remove(toRemove);
    }

    //public bool IsValidSelection(GameObject newSelection)
    //{

    //    foreach (GameObject selected in allSelectedObjects)
    //    {
    //        if(newSelection.GetHashCode() == selected.GetHashCode())
    //        {
    //            Debug.Log("Selection already in list, removing: " + newSelection);
    //            return false;
    //        }
    //    }

    //    return true;
    //}

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
        GameObject.Destroy(sel.transform.Find("Selection Indicator").gameObject);
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
