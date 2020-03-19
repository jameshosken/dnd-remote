using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.SceneManagement;

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

        socket = GetComponent<SocketIOComponent>();
        startServer = GetComponent<StartNodeServer>();

        //Check if connection exists:
        if (socket.IsConnected)
        {
            Debug.LogWarning("Already connected to socket");
            UpdateStatusMessage("Connected to Server", Color.green);
        }

        //If not, check if build or editor:
        else if (Application.isEditor)
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
            Debug.Log("Server connected at: " + filePath);
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


    //private void OnLevelWasLoaded(int level)
    //{
    //    //Todo: Make this nonesense more dynamic and scalable
    //    if (level == 1) //PLAYER
    //    {
    //        gameObject.AddComponent<NetworkPlayerMapUpdater>();
    //        gameObject.AddComponent<NetworkPlayerMeshUpdater>();
    //    }
    //    else if (level == 2)    //DM
    //    {
    //        gameObject.AddComponent<NetworkDMMapUpdater>();
    //        gameObject.AddComponent<NetworkDMMeshUpdater>();
    //    }
    //}


    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);
        if (scene.name == "Player Main") //PLAYER
        {
            gameObject.AddComponent<NetworkPlayerMapUpdater>();
            gameObject.AddComponent<NetworkPlayerMeshUpdater>();
        }
        else if (scene.name == "DM Main")    //DM
        {
            gameObject.AddComponent<NetworkDMMapUpdater>();
            gameObject.AddComponent<NetworkDMMeshUpdater>();
        }
    }
}
