using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreferencesHandler : MonoBehaviour
{
    public void UpdateName(string name)
    {
        PlayerPrefs.SetString("name", name);
    }
}
