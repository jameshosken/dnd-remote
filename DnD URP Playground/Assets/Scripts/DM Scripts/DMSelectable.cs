using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMSelectable : MonoBehaviour
{
    NetworkDMMapUpdater dmMapUpdater;
    NetworkPlayerMapUpdater selfMapUpdater;

    public SerializedTile tile;

    Material myMaterial;
    private void Start()
    {
    
        //Ensure obj has collider
        if (!gameObject.GetComponent<Collider>())
        {
            gameObject.AddComponent<BoxCollider>();
        }

        tile = new SerializedTile(gameObject.name, gameObject.GetHashCode());

        myMaterial = GetComponentInChildren<Renderer>().material;
        tile.materialName = myMaterial.name.Replace("(Instance)", "").Trim();
    }

    private void Update()
    {
        tile.location = transform.position;
        tile.rotation = transform.rotation.eulerAngles;

        //Doesn't work. Fix
        //Look for first material in children. If new, update myMaterial and tile material;
        if (myMaterial.name != GetComponentInChildren<Renderer>().material.name)
        {
            tile.materialName = myMaterial.name;
            Debug.LogWarning("Updating Tile Material: " + myMaterial.name);
        }
    }



}
