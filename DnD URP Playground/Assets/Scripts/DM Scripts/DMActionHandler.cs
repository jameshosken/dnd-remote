using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



//Handles all generic things an object can do (rotate, move, etc)
public class DMActionHandler : MonoBehaviour
{
    [SerializeField] GameObject movementIndicator;

    [SerializeField] GameObject deleteIndicator;

    DMInterfaceHandler interfaceHandler;
    DMSelectionHandler selectionHandler;
    Text infoText;

    //For Measuring:
    Vector3 p1 = Vector3.zero;
    LineRenderer lineRenderer;

    //For wall placing:
    int wallClickCounter = 0;

    void Start()
    {
        interfaceHandler = FindObjectOfType<DMInterfaceHandler>();
        selectionHandler = FindObjectOfType<DMSelectionHandler>();
        lineRenderer = GetComponent<LineRenderer>();

        infoText = GameObject.Find("InfoText").GetComponent<Text>();
    }

    void Update()
    {

        movementIndicator.transform.position = interfaceHandler.GetMouseGridPosition();
        deleteIndicator.transform.position = interfaceHandler.GetInsideMouseGridPosition();



        //Todo: Consolidate these indicators
        if (interfaceHandler.mode != DMInterfaceHandler.Mode.MOVE)
        {
            if (movementIndicator.activeSelf) movementIndicator.SetActive(false);
            if (lineRenderer.enabled) lineRenderer.enabled = false;
        }
        if (interfaceHandler.mode != DMInterfaceHandler.Mode.DELETE)
        {
            if (deleteIndicator.activeSelf) deleteIndicator.SetActive(false);
        }

        switch (interfaceHandler.mode)
        {
            case DMInterfaceHandler.Mode.MOVE:
                if(!movementIndicator.activeSelf) movementIndicator.SetActive(true);
                HandleMeasureTool();
                break;
            case (DMInterfaceHandler.Mode.DELETE):
                if (!deleteIndicator.activeSelf) deleteIndicator.SetActive(true);
                HandleSelectedObjectDelete();
                break;
            default:
                break;
        }

    }

    public void UpdateMeasureToolStart()
    {
        p1 = interfaceHandler.GetMouseGridPosition();
    }

    private void HandleMeasureTool()
    {
        
        if (!lineRenderer.enabled) lineRenderer.enabled = true;

        if (selectionHandler.selected != null) p1 = selectionHandler.selected.transform.position;
        
        Vector3 p2 = interfaceHandler.GetMouseGridPosition();

        lineRenderer.SetPosition(0, p1);
        lineRenderer.SetPosition(1, p2);

        float d = Vector3.Distance(p1, p2) * 5f;    //Size of each grid
        infoText.text = "Distance: " + d.ToString("F1");

        
    }

    public void HandleSelectedObjectRotate(int dir)
    {
        if(selectionHandler.selected != null) selectionHandler.selected.transform.Rotate(Vector3.up * 90 * dir);
    }

    private void HandleSelectedObjectDelete()
    {
        if (selectionHandler.selected != null)
        {
            GameObject toDestroy = selectionHandler.selected;
            selectionHandler.TryClearSelection();
            GameObject.Destroy(toDestroy);
        }
    }

    public void HandleSelectedObjectMove()
    {
        if(selectionHandler.selected != null)
        {
            selectionHandler.selected.transform.position = movementIndicator.transform.position;
        }
       
    }
}
