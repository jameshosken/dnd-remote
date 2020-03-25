using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles interaction with mouse and keyboard.
/// </summary>
public class DMInterfaceHandler : MonoBehaviour
{
    public enum Mode
    {
        SELECT,
        PLACE,
        MOVE,
        DELETE
    }

    [SerializeField] Dropdown selectionModeDropdown;
    [SerializeField] Dropdown objectPlacementDropdown;

    DMActionHandler actionHandler;
    DMCreationHandler creationHandler;
    DMSelectionHandler selectionHandler;
    GenericMaterialsHandler materialsHandler;

    int elevation = 0;

    public Mode mode = Mode.SELECT;
    public Vector3 lastClickedLocation = Vector3.zero;

    [Tooltip("Time in seconds between mouseDown and mouseUp in which a click will register")]
    [SerializeField] float clickThreshold = 0.1f;
    float timeAtMouseDown = 0f;

    private void Start()
    {
        actionHandler = FindObjectOfType<DMActionHandler>();
        creationHandler = FindObjectOfType<DMCreationHandler>();
        selectionHandler = FindObjectOfType<DMSelectionHandler>();
        materialsHandler = FindObjectOfType<GenericMaterialsHandler>();

        List<string> selectionNames = new List<string>();
        int c = 1;
        foreach (var m in Enum.GetNames(typeof(Mode)))
        {
            selectionNames.Add("(" + c.ToString() + ") " + m);
            c++;
        }
        selectionModeDropdown.AddOptions(selectionNames);

        selectionModeDropdown.onValueChanged.AddListener(delegate
        {
            ChangeMode(selectionModeDropdown.value);
        });
    }

    private void Update()
    {
        HandleElevationChangeKeypress(KeyCode.Equals, KeyCode.Minus);
        HandleObjectTypeChangeKeypress(KeyCode.T);
        HandleMouseModeChangeKeypress();
        HandleObjectRotation(KeyCode.Q, KeyCode.E);
        HandleMaterialChange(KeyCode.M);

        HandlePrimaryMouse();
        HandleSecondaryMouseClick();

    }



    //Fired by dropdown
    public void ChangeMode(int m)
    {
        //Changes mouse interaction mode
        mode = (Mode)m;
        Debug.Log("Mode Changed: " + mode);
    }

    public void ChangeModeByName(Mode m)
    {
        mode = m;
        Debug.Log("Mode Changed By Name: " + mode);
    }

    private void HandleMaterialChange(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            materialsHandler.CycleMaterial();
        }
    }

    private void HandleSecondaryMouseClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(mode == DMInterfaceHandler.Mode.MOVE)actionHandler.HandleSelectedObjectMove();
        }
    }

    private void HandlePrimaryMouse()
    {

        

        if (Input.GetMouseButtonDown(0))
        {
            timeAtMouseDown = Time.time;

            if (mode == DMInterfaceHandler.Mode.PLACE)
            {
                creationHandler.OnBeginDrag();
            }

            if (mode == DMInterfaceHandler.Mode.SELECT ||
                    mode == DMInterfaceHandler.Mode.MOVE ||
                    mode == DMInterfaceHandler.Mode.DELETE)
            {
                selectionHandler.OnBeginDrag();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {

            

            if (mode == DMInterfaceHandler.Mode.PLACE)
            {
                creationHandler.OnEndDrag();
            }

            if (mode == DMInterfaceHandler.Mode.SELECT ||
                    mode == DMInterfaceHandler.Mode.MOVE ||
                    mode == DMInterfaceHandler.Mode.DELETE)
            {
                selectionHandler.OnEndDrag();
            }

            if (Time.time - timeAtMouseDown < clickThreshold)
            {
                OnMouseClick();
            }


        }



    }

    private void OnMouseClick()
    {

        actionHandler.UpdateMeasureToolStart();

        //Applies to all modes, AFTER all above functions:
        lastClickedLocation = GetMouseGridPosition();

        if (mode == DMInterfaceHandler.Mode.SELECT ||
                    mode == DMInterfaceHandler.Mode.MOVE ||
                    mode == DMInterfaceHandler.Mode.DELETE)
        {
            selectionHandler.HandleSelectionClick();
        }

    }

    private void HandleObjectRotation(KeyCode ccw, KeyCode cw)
    {
        if (Input.GetKeyDown(ccw))
        {
            actionHandler.HandleSelectedObjectRotate(-1);
            creationHandler.RotatePlacement(-1);
        }
        else if (Input.GetKeyDown(cw))
        {
            actionHandler.HandleSelectedObjectRotate(1);
            creationHandler.RotatePlacement(1);
        }
    }

    private void HandleMouseModeChangeKeypress()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectionModeDropdown.value = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectionModeDropdown.value = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectionModeDropdown.value = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectionModeDropdown.value = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectionModeDropdown.value = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectionModeDropdown.value = 5;

        }
    }

    private void HandleObjectTypeChangeKeypress(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            int n = (objectPlacementDropdown.value + 1) % objectPlacementDropdown.options.Count;
            objectPlacementDropdown.value = n;

        }
    }


    public Vector3 GetMouseGridPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point + hit.normal * 0.1f;// Hack for now: puts green thing on top/next to objects
            point.x = Mathf.Round(point.x);
            point.y = Mathf.Floor(point.y + elevation);
            point.z = Mathf.Round(point.z);
            //placementIndicator.transform.position = point;

            return point;
        }
        return Vector3.zero;
    }

    public Vector3 GetInsideMouseGridPosition()
    {
        //Returns point inside object you hover over
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point - hit.normal * 0.1f;// Hack for now: puts green thing on top/next to objects
            point.x = Mathf.Round(point.x);
            point.y = Mathf.Floor(point.y + elevation);
            point.z = Mathf.Round(point.z);
            //placementIndicator.transform.position = point;

            return point;
        }
        return Vector3.zero;
    }

    void HandleElevationChangeKeypress(KeyCode up, KeyCode down)
    {
        if (Input.GetKeyDown(up))
        {
            elevation++;
        }
        if (Input.GetKeyDown(down))
        {
            if (elevation > 0)
            {
                elevation--;
            }
        }
    }
}
