using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This class takes stores GameObject info in a format that can send over socketIO.
[Serializable]
public class SerializedTile
{

    public bool hidden = false;
    public string prefabName; 
    public int uid;
    public Vector3 location;
    public Vector3 rotation;

    public SerializedTile(string name, int _uid)
    {
        prefabName = name;
        uid = _uid;
    }

}
