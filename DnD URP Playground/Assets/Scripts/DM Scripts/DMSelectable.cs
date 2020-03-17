using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMSelectable : MonoBehaviour
{
    UpdateClientMaps dmMapUpdater;
    UpdateSelfMap selfMapUpdater;

    public TileBlock tile;

    //PlayerInfo pInfo;
    private void Start()
    {
        //gameObject.AddComponent<TileBlock>();

        if (!gameObject.GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
        }

        tile = new TileBlock(gameObject.name, gameObject.GetHashCode());

    }

    private void Update()
    {
        tile.location = transform.position;
        tile.rotation = transform.rotation.eulerAngles;
    }

}
