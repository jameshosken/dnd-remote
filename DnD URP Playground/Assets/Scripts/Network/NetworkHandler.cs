using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

[RequireComponent(typeof(StartNodeServer))]
[RequireComponent(typeof(SocketIOComponent))]

public class NetworkHandler : MonoBehaviour
{
    [SerializeField] Text status;
    StartNodeServer startServer;
    SocketIOComponent socket;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(status.gameObject);

        socket = GetComponent<SocketIOComponent>();
        startServer = GetComponent<StartNodeServer>();

        //Check if connection exists:
        if (socket.IsConnected)
        {
            UpdateStatusMessage("Connected to Server", Color.green);
        }

        //If not, check if build or editor:
        if (Application.isEditor)
        {

            TryStartServer(Application.dataPath + "/../../server/app.js");
            


        }
        else
        {
            //if build, do nothing. Requires manual server input
        }


    }

    public void TryStartServer(string filePath)
    {
        if (startServer.StartServerProcess(filePath))
        {
            UpdateStatusMessage("Connected to Server", Color.green);
        }
        else
        {
            UpdateStatusMessage("Problem connecting to server ¯\\_(ツ)_/¯", Color.red);
        }
    }

    void UpdateStatusMessage(string msg, Color col)
    {
        if (status != null)
        {
            status.text = msg;
            status.color = col;
        }
    }


    private void OnLevelWasLoaded(int level)
    {
        //Todo: Make this nonesense more dynamic and scalable
        if (level == 1) //PLAYER
        {
            gameObject.AddComponent<NetworkPlayerMapUpdater>();
            gameObject.AddComponent<NetworkPlayerMeshUpdater>();
        }
        else if (level == 2)    //DM
        {
            gameObject.AddComponent<NetworkDMMapUpdater>();
            gameObject.AddComponent<NetworkDMMeshUpdater>();
        }
    }
}
