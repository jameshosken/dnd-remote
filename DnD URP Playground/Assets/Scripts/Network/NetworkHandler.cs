using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(StartNodeServer))]
[RequireComponent(typeof(SocketIOComponent))]

public class NetworkHandler : MonoBehaviour
{
    [SerializeField] Text status;
    [SerializeField] InputField serverIDInput;

    StartNodeServer startServer;
    SocketIOComponent socket;

    bool hasNgrokURL = false;

    int role = 0; //player = 0, dm = 1; Todo: this is dumb, make this a singular system accessible to all scripts

    string serverID = "";
    string serverPath = "";

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        socket = GetComponent<SocketIOComponent>();
        startServer = GetComponent<StartNodeServer>();

    }

    //Fired by role dropdown
    public void OnRoleChange(int r)
    {
        role = r;
    }

    public void OnStartServerClicked()
    {
        if(role == 0)
        {
            StartPlayerNetworkHandler();
        }else if(role == 1)
        {
            StartDMNetworkHandler();
        }
    }

    private void StartPlayerNetworkHandler()
    {
        serverID = serverIDInput.text;

        if(serverID != "")
        {
            socket.url = "ws://" + serverID + ".ngrok.io/socket.io/?EIO=4&transport=websocket";

            socket.Connect();

            Debug.Log("Socket URL:");
            Debug.Log(socket.url);
        }
        else
        {
            Debug.LogError("ServerID not valid");
        }
    }

    public void OnServerFileDropped(string file)
    {
        serverPath = file;
    }

    public void StartDMNetworkHandler()
    {

        //Check if connection exists:
        if (socket.IsConnected)
        {
            Debug.LogWarning("Already connected to socket");
            UpdateStatusMessage("Connected to Server", Color.green);
        }

        //If not, check if build or editor:
        if (Application.isEditor)
        {
            TryStartServer(Application.dataPath + "/../../server/app.js");
        }
        else
        {
            TryStartServer(serverPath);
        }

        //Start pinging server for url
        StartCoroutine(RequestNewNgrokURL());
        socket.On("ngrok-url", OnNewNgrokURL);
    }

    void OnNewNgrokURL(SocketIOEvent data)
    {
        string url = data.data.GetField("url").ToString();

        if (url != "")
        {
            hasNgrokURL = true;

            string[] chunk = url.Split('.');
            string id = chunk[0].Split(new string[] { "//" }, System.StringSplitOptions.None)[1];
            UpdateStatusMessage("Your server ID: " + id, Color.green);
        }

    }

    IEnumerator RequestNewNgrokURL()
    {
        if (!socket.IsConnected) yield return null;
        

        while(!hasNgrokURL){
            socket.Emit("request-ngrok-url");
            yield return new WaitForSeconds(3);
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
