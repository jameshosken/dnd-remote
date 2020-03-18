using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenHandler : MonoBehaviour
{
    [SerializeField] int playerScene;
    [SerializeField] int DMScene;

    [Space]
    [Header("UI Attributes")]
    [SerializeField] Dropdown playerType;
    [SerializeField] InputField playerName;
    [Space]
    [Header("Network Attributes")]
    [SerializeField] NetworkHandler network;

    public void StartScene()
    {

        string name = playerName.text;

        GetComponent<PlayerPreferencesHandler>().UpdateName(name);

        //Todo: link this is to DM/PLAYER enum
        int type = playerType.value;
        if(type == 0)
        {
            SceneManager.LoadScene(playerScene);
        }else if(type == 1)
        {
            
            SceneManager.LoadScene(DMScene);
        }
    }

}
