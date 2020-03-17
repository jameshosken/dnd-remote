using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMCameraController : MonoBehaviour
{
    
    [SerializeField] Transform cameraObject;
    [SerializeField] Transform cameraPivot;

    [Tooltip("x for yaw, y for pitch")]
    [SerializeField] float movementMultiplier = 1f;
    [SerializeField] Vector2 rotationMultiplier = new Vector2(1, 1);
    [SerializeField] float scrollZoomMultiplier = 1;
    [SerializeField] Vector2 cameraTiltLimits = new Vector2(10, 80);
    [SerializeField] float cameraYLimit = 1;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            HandleMouseRotation();
        }

        HandleScroll();


        HandleMove();

    }

    private void HandleMove()
    {
        //Hacky way to find XZ forward direction of camera
        Vector3 forward = Vector3.Scale(cameraPivot.forward, new Vector3(1, 0, 1)).normalized;

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * movementMultiplier;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * movementMultiplier;


        //We're moving the whole camera rig here:'
        transform.Translate(forward * z);
        transform.Translate(cameraPivot.right * x);


    }

    private void HandleScroll()
    {
        Vector2 scroll = Input.mouseScrollDelta;

        float y = scroll.y;

        //Prevent cam going throuh ground
        if ((cameraObject.transform.position + cameraObject.transform.forward * y).y > cameraYLimit)
        {
            cameraObject.transform.Translate(0, 0, y, Space.Self);
        }

    }

    private void HandleMouseRotation()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        cameraPivot.Rotate(0, x * rotationMultiplier.x, 0, Space.World);



        //Ensure pivot doesn't rotate beyond limits:
        float newRotX = cameraPivot.rotation.eulerAngles.x + y * rotationMultiplier.y;
        if (newRotX > cameraTiltLimits.x && newRotX < cameraTiltLimits.y)
        {
            cameraPivot.Rotate(y * rotationMultiplier.y, 0, 0, Space.Self);
        }
    }
}
