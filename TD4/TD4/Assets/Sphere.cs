using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Sphere : MonoBehaviour
{
    private float rayon = 1.0f; //Rajouter une prise en compte du rayon dans le code car pour l'instant == 1 et c'est tout

    // Start is called before the first frame update
    void Start()
    {
        CreateSphere();
    }

    private void CreateSphere()
    {
        int resX = 15;
        int resY = 15;
        float pi = Mathf.PI;
        float teta = -1;
        float phi = -pi / 2;
        float interX = 2 * pi / (resX - 1);
        float interY = pi / (resY - 1);

        float x, y, z;
        float n1, n2, n3, n4;

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(Mathf.Cos(2 * pi) * Mathf.Cos(pi / 2), Mathf.Sin(2 * pi) * Mathf.Cos(pi / 2), Mathf.Sin(pi / 2)));

        //Vertices
        while(teta <= 2*pi)
        {
            x = Mathf.Cos(teta) * Mathf.Cos(phi);
            y = Mathf.Sin(teta) * Mathf.Cos(phi);
            z = Mathf.Sin(phi);

            vertices.Add(new Vector3(x, y, z));

            while(phi <= pi/2)
            {
                x = Mathf.Cos(teta) * Mathf.Cos(phi);
                y = Mathf.Sin(teta) * Mathf.Cos(phi);
                z = Mathf.Sin(phi);

                vertices.Add(new Vector3(x, y, z));

                phi += interY;
            }
            teta += interX;
            phi = -pi / 2;
        }

        List<int> triangles = new List<int>();

        //Triangle
        for (int i = 0; i < resX; i++)
        {
            for (int j = 0; j < resY; j++)
            {
                n1 = i * resY + j;
                n2 = (i + 1) * resY + j;
                n3 = (i + 1) * resY + (j + 1);
                n4 = i * resY + (j + 1);

                triangles.Add(Convert.ToInt32(n1));
                triangles.Add(Convert.ToInt32(n2));
                triangles.Add(Convert.ToInt32(n3));

                triangles.Add(Convert.ToInt32(n1));
                triangles.Add(Convert.ToInt32(n3));
                triangles.Add(Convert.ToInt32(n4));
            }
        }

        //Create 
        Mesh mesh = new Mesh();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    public float getRayon()
    {
        return rayon;
    }
}
