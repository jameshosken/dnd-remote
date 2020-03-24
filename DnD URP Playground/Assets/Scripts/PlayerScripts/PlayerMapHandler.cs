using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapHandler : MonoBehaviour
{
    [SerializeField] Transform hiddenStorage;
    [SerializeField] Transform mapContainer;
    [SerializeField] Material newObjMaterial;

    List<GameObject> creatableObjects = new List<GameObject>();


    GenericMaterialsHandler materialsHandler;
    private void Start()
    {
        materialsHandler = GetComponent<GenericMaterialsHandler>();
        LoadPrefabsIntoScene();
    }

    public void DeleteGameObjectFromScene(SerializedTile currTile, Map currentMap)
    {
        //Debug.LogWarning("Deleting GO From Scene");
        GameObject tileToDelete = GameObject.Find(NetworkPlayerMapUpdater.GetNameFromTileUID(currTile));
        currentMap.RemoveTile(currTile);
        GameObject.Destroy(tileToDelete);
    }

    public void CreateNewGameobjectFromTile(SerializedTile newTile, Map currentMap)
    {
        string newTileName = newTile.prefabName;

        foreach (GameObject prefab in creatableObjects)
        {
            if (prefab.name == newTileName)
            {
                GameObject clone = Instantiate(prefab, newTile.location, Quaternion.Euler(newTile.rotation));
                clone.name = NetworkPlayerMapUpdater.GetNameFromTileUID(newTile);
                clone.transform.parent = this.transform;

                Renderer[] renderers = clone.GetComponentsInChildren<Renderer>();
                Material m = materialsHandler.GetMaterialByName(newTile.materialName);
                foreach (Renderer renderer in renderers)
                {
                    renderer.material = m;
                }

                currentMap.AddTile(newTile);
                return;
            }
            else
            {
            }
        }
    }


    //UTILITY
    private void LoadPrefabsIntoScene()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");
        foreach (GameObject prefab in prefabs)
        {
            GameObject clone = Instantiate(prefab) as GameObject;
            clone.name = prefab.name;
            clone.transform.parent = hiddenStorage;
            creatableObjects.Add(clone);
        }
    }
    public void CreateNewHiddenPrefabFromMesh(Mesh mesh)
    {
        GameObject go = new GameObject();
        //Addcomponenet handily returns new comopnent
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = newObjMaterial;
        go.name = mesh.name;
        go.transform.parent = hiddenStorage;
        creatableObjects.Add(go);
        Debug.Log("New GO Prefab Added!");
    }
}
