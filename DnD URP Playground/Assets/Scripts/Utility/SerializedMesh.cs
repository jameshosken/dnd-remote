using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializedMesh
{
    public string name;
    public Vector3[] verts;
    public Vector2[] uv;
    public int[] tris;
    public SerializedMesh(Mesh mesh)
    {
        name = mesh.name;
        verts = mesh.vertices;
        uv = mesh.uv;
        tris = mesh.triangles;
    }
}

public class SerializedMeshArray
{
    public List<SerializedMesh> meshes = new List<SerializedMesh>();
}