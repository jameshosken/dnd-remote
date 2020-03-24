using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(SocketIOComponent))]

public class NetworkHandler : MonoBehaviour
{
    [SerializeField] Text status;
    [SerializeField] InputField serverIDInput;

    SocketIOComponent socket;

    int role = 0; //player = 0, dm = 1; Todo: this is dumb, make this a singular system accessible to all scripts

    string serverID = "";
    string serverPath = "https://closed-protective-pudding.glitch.me/";

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        socket = GetComponent<SocketIOComponent>();

    }

    //Fired by role dropdown
    public void OnRoleChange(int r)
    {
        role = r;
    }

    public void OnStartServerClicked()
    {
        socket.url = serverPath;
        socket.Connect();
        socket.On("hello", OnHello);
        Debug.LogWarning("Socket URL:");
        Debug.LogWarning(socket.url);
        return;


    }

    private void OnHello(SocketIOEvent obj)
    {
        UpdateStatusMessage("Connection Successful!", Color.green);
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
