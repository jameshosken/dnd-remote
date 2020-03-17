using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImportObjectFromFile : MonoBehaviour
{

    [SerializeField] Material newObjectMaterial;

    OBJChunkImporter oi;

    public List<GameObject> importedCollection = new List<GameObject>();

    [SerializeField] GameObject ObjectCreationHandler;

    Text infoText;

    private void Start()
    {
        oi = GetComponent<OBJChunkImporter>();
        infoText = GameObject.Find("InfoText").GetComponent<Text>();
    }

    public void TryImportFile(string filePath)
    {
        // TODO: find user friendly fild upload system    
        Debug.Log("Loading File:");
        Debug.Log(filePath);
        oi.ImportFile(filePath);
    }

    public void OnReceiveMeshData(Mesh newMesh)
    {

        CreateGameObjectFromMesh(newMesh);
        Debug.Log("Sending Mesh to Client:");
        FindObjectOfType<UpdateClientMeshes>().SendMeshToClients(newMesh);
        Debug.Log("Mesh sent to Client:");
        
    }

    public void CreateGameObjectFromMesh(Mesh newMesh)
    {
        GameObject newObj = new GameObject(newMesh.name);

        MeshFilter mF = newObj.AddComponent<MeshFilter>();
        MeshRenderer mR = newObj.AddComponent<MeshRenderer>();
        mF.mesh = newMesh;
        mR.material = newObjectMaterial;

        importedCollection.Add(newObj);

        ObjectCreationHandler.SendMessage("OnNewPlaceableObject", newObj);

        infoText.text = ("Successfully added: " + newObj.name);
    }
}
