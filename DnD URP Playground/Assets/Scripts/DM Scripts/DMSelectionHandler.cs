using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Script handles selection
public class DMSelectionHandler : MonoBehaviour
{
    public GameObject selected = null;
    GameObject selectedIndicator = null;

    Camera main;

    Material selectionMaterial;


    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        selectionMaterial = Resources.Load("SelectionMaterial") as Material;
    }


    private void HandleSecondaryMouseClick()
    {
        throw new NotImplementedException();
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
                //See if there's a better way to get parent of clicked object
                GameObject DMSelection = clicked.GetComponentInParent<DMSelectable>().gameObject;

                isSuccessfulSelection = TryNewSelection(DMSelection);

            }
        }

        if (!isSuccessfulSelection)
        {
            TryClearSelection();
        }
    }

    public void TryClearSelection()
    {
        TryDeactivateShader(selected);
        selected = null;
        Debug.Log("Cleared Selection");
    }

    public bool TryNewSelection(GameObject newSelection)
    {
        if (selected == newSelection)
        {
            return false;
        }

        TryDeactivateShader(selected);
        selected = newSelection;
        ActivateSelectionShader(selected);
        Debug.Log("New Selection: " + selected.name);
        return true;
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
        selectedIndicator.transform.parent = sel.transform;
        selectedIndicator.name = "TEMP SELECTION OBJECT";

        Renderer[] renderers = selectedIndicator.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = selectionMaterial;
        }
    }

    void DeactivateSelectionShader(GameObject sel)
    {
        GameObject.Destroy(selectedIndicator);
    }
}
