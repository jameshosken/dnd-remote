using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMSelectable : MonoBehaviour
{
    NetworkDMMapUpdater dmMapUpdater;
    NetworkPlayerMapUpdater selfMapUpdater;

    public SerializedTile tile;

    private void Start()
    {
    
        //Ensure obj has collider
        if (!gameObject.GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
        }

        tile = new SerializedTile(gameObject.name, gameObject.GetHashCode());

    }

    private void Update()
    {
        tile.location = transform.position;
        tile.rotation = transform.rotation.eulerAngles;
    }

}
