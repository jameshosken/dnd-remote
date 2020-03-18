using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class NetworkPlayerMeshUpdater : MonoBehaviour
{
    PlayerMapHandler mapHandler;
    SocketIOComponent socket;

    List<string> meshNames = new List<string>();

    private void Start()
    {
        mapHandler = FindObjectOfType<PlayerMapHandler>();
        socket = GetComponent<SocketIOComponent>();

        Invoke("RequestMeshData", 1);

        socket.On("update-mesh", ParseSingleMesh);

        socket.On("update-mesh-all", ParseBulkMesh);
    }
    void RequestMeshData()
    {
        socket.Emit("request-mesh-data", new JSONObject());
    }

    private void ParseSingleMesh(SocketIOEvent obj)
    {
        SerializedMesh sMesh = JsonUtility.FromJson<SerializedMesh>(obj.data.ToString());
        AddNewMesh(sMesh);
    }

    private void AddNewMesh(SerializedMesh sMesh)
    {
        //Don't add meshes that exist!
        if (meshNames.Contains(sMesh.name)) return; 

        Mesh newMesh = new Mesh();

        newMesh.vertices = sMesh.verts;
        newMesh.triangles = sMesh.tris;
        newMesh.uv = sMesh.uv;
        newMesh.name = sMesh.name;

        newMesh.RecalculateNormals();
        mapHandler.CreateNewHiddenPrefabFromMesh(newMesh);
        meshNames.Add(sMesh.name);
    }

    void ParseBulkMesh(SocketIOEvent obj)
    {
        Debug.Log("Bulk Mesh Data!");
        SerializedMeshArray sMeshArray = JsonUtility.FromJson<SerializedMeshArray>(obj.data.ToString());

        foreach (SerializedMesh sMesh in sMeshArray.meshes)
        {
            Debug.Log("Parsing Mesh: " + sMesh.name);
            AddNewMesh(sMesh);
        }
    }
}
