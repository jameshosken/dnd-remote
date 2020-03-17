using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TileBlock
{

    public bool hidden = false;
    public string prefabName; 
    public int uid;
    public Vector3 location;
    public Vector3 rotation;

    public TileBlock(string name, int _uid)
    {
        prefabName = name;
        uid = _uid;
    }

}
