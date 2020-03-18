using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapHandler : MonoBehaviour
{
    [SerializeField] Transform hiddenStorage;
    [SerializeField] Material newObjMaterial;
    List<GameObject> creatableObjects = new List<GameObject>();

    private void Start()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");

        Debug.Log("Loaded: " + prefabs);
        foreach (GameObject prefab in prefabs)
        {
            Debug.Log("Creating: " + prefab);
            GameObject clone = Instantiate(prefab) as GameObject;

            Debug.Log("Renaming: " + clone.name);
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

    public void DeleteGameObjectFromScene(SerializedTile currTile, Map currentMap)
    {
        Debug.LogWarning("Deleting GO From Scene");
        GameObject tileToDelete = GameObject.Find(NetworkPlayerMapUpdater.GetNameFromTileUID(currTile));
        currentMap.RemoveTile(currTile);
        GameObject.Destroy(tileToDelete);
    }

    public void CreateNewGameobjectFromTile(SerializedTile newTile, Map currentMap)
    {
        Debug.LogWarning("Creating New GO From Tile");

        string newTileName = newTile.prefabName;

        foreach (GameObject prefab in creatableObjects)
        {
            if (prefab.name == newTileName)
            {
                GameObject clone = Instantiate(prefab, newTile.location, Quaternion.Euler(newTile.rotation));
                clone.name = NetworkPlayerMapUpdater.GetNameFromTileUID(newTile);
                currentMap.AddTile(newTile);
                return;
            }
            else
            {
                Debug.LogWarning("New tile name not in list of creatable objects");
            }
        }
        Debug.LogWarning("No tile/object name match :(");
    }
}
