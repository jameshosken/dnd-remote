using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DisplayCurrentPlacementObject_August : MonoBehaviour
{
    [SerializeField] Material ghostMaterial;
    [SerializeField] Dropdown objectPlacementDropdown;
    DMCreationHandler_August creationHandler;

    GameObject display;

    // Start is called before the first frame update
    void Start()
    {
        creationHandler = GetComponentInParent<DMCreationHandler_August>();
        objectPlacementDropdown.onValueChanged.AddListener(delegate
        {
            OnDropdownUpdate();
        });

        OnDropdownUpdate();
    }

    private void OnDropdownUpdate()
    {
        GameObject.Destroy(display);
        GameObject displayPrefab = creationHandler.placementObjects[creationHandler.objectIdxToCreate];

        display = Instantiate(displayPrefab, transform.position, transform.rotation);
        display.transform.parent = transform;

        Renderer[] renderers = display.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            r.material = ghostMaterial;
        }

        Collider[] cols = display.GetComponentsInChildren<Collider>();

        foreach (Collider c in cols)
        {
            c.enabled = false;
        }

        //Ensure data is not sent to server
        DMSelectable[] sel = display.GetComponentsInChildren<DMSelectable>();
        foreach (DMSelectable s in sel)
        {
            s.enabled = false;
        }

    }


}
