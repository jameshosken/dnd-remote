using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DMMeasureHandler : MonoBehaviour
{

    DMInterfaceHandler_August interfaceHandler;

    DMSelectionHandler selectionHandler;

    LineRenderer lineRenderer;

    Text infoText;
    
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
        
    }
}
