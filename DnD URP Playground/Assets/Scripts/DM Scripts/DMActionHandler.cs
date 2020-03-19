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

    DMInterfaceHandler_August interfaceHandler;
    DMSelectionHandler selectionHandler;
    Text infoText;

    //For Measuring:
    Vector3 p1 = Vector3.zero;
    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        interfaceHandler = FindObjectOfType<DMInterfaceHandler_August>();
        selectionHandler = FindObjectOfType<DMSelectionHandler>();
        lineRenderer = GetComponent<LineRenderer>();

        infoText = GameObject.Find("InfoText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 movepos = interfaceHandler.GetMouseGridPosition();
        Vector3 delpos = interfaceHandler.GetInsideMouseGridPosition();
        movementIndicator.transform.position = movepos;
        deleteIndicator.transform.position = delpos;

        if (interfaceHandler.mode != DMInterfaceHandler_August.Mode.MOVE)
        {
            if (movementIndicator.activeSelf) movementIndicator.SetActive(false);
            if (lineRenderer.enabled) lineRenderer.enabled = false;
        }
        if (interfaceHandler.mode != DMInterfaceHandler_August.Mode.DELETE)
        {
            if (deleteIndicator.activeSelf) deleteIndicator.SetActive(false);
        }
        switch (interfaceHandler.mode)
        {
            case DMInterfaceHandler_August.Mode.MOVE:
                if(!movementIndicator.activeSelf) movementIndicator.SetActive(true);
                HandleSelectedObjectMove(movepos);
                HandleMeasureTool(movepos);
                break;
            case (DMInterfaceHandler_August.Mode.DELETE):

                if (!deleteIndicator.activeSelf) deleteIndicator.SetActive(true);
                HandleSelectedObjectDelete();
                break;
            default:
                break;
        }

    }

    private void HandleMeasureTool(Vector3 movePos)
    {
        
        if (!lineRenderer.enabled) lineRenderer.enabled = true;

        
        if (selectionHandler.selected != null) p1 = selectionHandler.selected.transform.position;
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                p1 = interfaceHandler.GetMouseGridPosition();
            }
        }

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

    private void HandleSelectedObjectMove(Vector3 pos)
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(selectionHandler.selected != null)
            {
                selectionHandler.selected.transform.position = pos;
            }
        }
        
    }
}
