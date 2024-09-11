using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylindre : MonoBehaviour
{

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<MeshFilter>().mesh = mesh;

        newVertices = new Vector3[4];

        newVertices[0] = new Vector3(0, 0, 0);
        newVertices[1] = new Vector3(0, 1, 0);
        newVertices[2] = new Vector3(1, 1, 0);
        newVertices[3] = new Vector3(1, 0, 0);

        newTriangles = new int[] { 0, 1, 2, 0, 2, 3 };

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }
}
