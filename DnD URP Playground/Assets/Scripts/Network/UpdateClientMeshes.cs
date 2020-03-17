using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class UpdateClientMeshes : MonoBehaviour
{


    SerializedMeshArray meshes = new SerializedMeshArray();
    SocketIOComponent socket;

    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.On("request-mesh-data", RespondToMeshRequest);
    }


    public void SendMeshToClients(Mesh mesh)
    {

        SerializedMesh smesh = new SerializedMesh(mesh);
        var json_mesh = JSONObject.Create(JsonUtility.ToJson(smesh));
        
        socket.Emit("dm-update-mesh", json_mesh);

        meshes.meshes.Add(smesh);
    }

    void RespondToMeshRequest(SocketIOEvent obj)
    {
        SendBulkMeshesToClients();
    }
    public void SendBulkMeshesToClients()
    {
        var json_mesh = JSONObject.Create(JsonUtility.ToJson(meshes));

        socket.Emit("dm-update-mesh-all", json_mesh);
    }   
}


