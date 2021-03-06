﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class NetworkPlayerMapUpdater : MonoBehaviour
{
    public List<SerializedTile> tiles;

    PlayerMapHandler mapHandler;

    SocketIOComponent socket;
    Map newMap;
    Map currentMap;


    void Start()
    {

        mapHandler = FindObjectOfType<PlayerMapHandler>();
        socket = GetComponent<SocketIOComponent>();
        newMap = new Map();
        currentMap = new Map();

        socket.On("update-map", UpdateMap);
    }

   

    private void UpdateMap(SocketIOEvent obj)
    {
        newMap = JsonUtility.FromJson<Map>(obj.data.ToString());
        CompareMaps();
    }

    private void CompareMaps()
    {
        //omg so clunky

        //Determine if new GameObject required:
        for (int i = 0; i < newMap.tiles.Count; i++)
        {
            bool contains = false;

            foreach (SerializedTile currTile in currentMap.tiles)
            {
                if (CompareTiles(currTile, newMap.tiles[i]))
                {
                    //Determine if location must be updated:
                    UpdateTileLocation(currTile, newMap.tiles[i]);
                    contains = true;
                    break;
                }
            }

            if (contains) continue; // Do nothing if correct tile exists
            else mapHandler.CreateNewGameobjectFromTile(newMap.tiles[i], currentMap);
        }

        //Determine if old GameObject must be removed:
        for (int i = currentMap.tiles.Count - 1; i >= 0; i--) { 
            SerializedTile currTile = currentMap.tiles[i];
            bool contains = false;
            foreach(SerializedTile newTile in newMap.tiles)
            {
                if(CompareTiles(currTile, newTile)){
                    contains = true;
                    break;
                }
            }

            if (contains)continue; // Do nothing if tile is supposed to be here
            else mapHandler.DeleteGameObjectFromScene(currTile, currentMap);
        }

        
    }

    private void UpdateTileLocation(SerializedTile currTile, SerializedTile newTile)
    {
        if(CompareTilePSR(currTile, newTile))
        {
            return;
        }
        else
        {
            GameObject.Find(GetNameFromTileUID(currTile)).transform.position = newTile.location;

            GameObject.Find(GetNameFromTileUID(currTile)).transform.rotation = Quaternion.Euler(newTile.rotation);
        }
    }


    public static string GetNameFromTileUID(SerializedTile tile)
    {
        return tile.uid.ToString();
    }

    bool CompareTiles(SerializedTile t1, SerializedTile t2)
    {
        return t1.uid == t2.uid;
    }

    bool CompareTilePSR(SerializedTile t1, SerializedTile t2)
    {

        bool loc = (Vector3.Distance(t1.location, t2.location) < 0.1f);

        bool rot = Vector3.Angle(t1.rotation, t2.rotation) < 0.05f;

        if (loc && rot)
        {
            return true;
        }
        return false;
    }


}
