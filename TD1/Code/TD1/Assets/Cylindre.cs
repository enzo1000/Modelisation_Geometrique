using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Cylindre : MonoBehaviour
{
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    // Start is called before the first frame update
    void Start()
    {
        //Create2x2Plane();
        //CreateCylinder();
        //CreateSphere();
        //createCone();
        PacMan();
    }

    private void PacMan()
    {
        //Résolution & taille de la sphère
        int nbParalleles = 19;
        int nbMeridiens = 19;
        int radius = 5;

        //Angle bouche pac-man
        float angleTronquage = 50;

        List<Vector3> points = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        if (nbParalleles == 0 || nbMeridiens == 0)
        {
            Debug.LogError("Nb paralleles ou Nb meridiens est vide.");
            return;
        }

        //Résolution verticale & horizontale
        float theta = 360 / nbMeridiens;
        float phi = 180 / nbParalleles;

        //Position des point haut et bas de la sphere
        int indNorthPole = points.Count;
        points.Add(new(0, radius, 0));
        int indSouthPole = points.Count;
        points.Add(new(0, -radius, 0));

        for (float i = -90 + phi; i < 90 - phi; i += phi)
        {
            for (float j = angleTronquage / 2; j < 360; j += theta)
            {
                //Si on est dans l'angle de la bouche de pac man
                if ((360 - angleTronquage / 2) > j && (360 - angleTronquage / 2 > j + theta))
                {
                    //Création des points pour les triangles
                    int t1 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos((j * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius,
                        Mathf.Sin(i * Mathf.PI / 180) * radius,
                        Mathf.Sin((j * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius));

                    int t2 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos((j * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin((j * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius));

                    int t3 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos(((j + theta) * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius,
                        Mathf.Sin(i * Mathf.PI / 180) * radius,
                        Mathf.Sin(((j + theta) * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius));

                    int t4 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos(((j + theta) * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin(((j + theta) * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius));

                    //Positionnement triangles additionnel
                    if (i != 90 - phi)
                    {
                        triangles.AddRange(new int[] { t1, t2, t3 });
                        triangles.AddRange(new int[] { t4, t3, t2 });
                    }

                    //Positionnement triangle pole sud
                    if (i == (-90 + phi))
                    {
                        triangles.AddRange(new int[] { t1, t3, indSouthPole });
                    }

                    //Positionnement triangle pole nord
                    if (i >= 90 - (2 * phi))
                    {
                        triangles.AddRange(new int[] { t2, indNorthPole, t4 });
                    }
                }
                else if ((360 - angleTronquage / 2) > j && !(360 - angleTronquage / 2 > j + theta))
                {
                    int t1 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos((j * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius,
                        Mathf.Sin(i * Mathf.PI / 180) * radius,
                        Mathf.Sin((j * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius));

                    int t2 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos((j * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin((j * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius));

                    int t3 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos(((360 - angleTronquage / 2) * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius,
                        Mathf.Sin(i * Mathf.PI / 180) * radius,
                        Mathf.Sin(((360 - angleTronquage / 2) * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius));

                    int t4 = points.Count;
                    points.Add(new Vector3(
                        Mathf.Cos(((360 - angleTronquage / 2) * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin((i + phi) * Mathf.PI / 180) * radius,
                        Mathf.Sin(((360 - angleTronquage / 2) * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius));

                    //Positionnement triangles additionnel
                    if (i != 90 - phi)
                    {
                        triangles.AddRange(new int[] { t1, t2, t3 });
                        triangles.AddRange(new int[] { t4, t3, t2 });
                    }

                    //Positionnement triangle pole sud
                    if (i == (-90 + phi))
                    {
                        triangles.AddRange(new int[] { t1, t3, indSouthPole });
                    }

                    //Positionnement triangle pole nord
                    if (i >= 90 - (2 * phi))
                    {
                        triangles.AddRange(new int[] { t2, indNorthPole, t4 });
                    }
                }
            }
        }

        //Comportement normal création de la sphère
        if (angleTronquage != 0)
        {
            float startAngle = (360 - angleTronquage / 2);
            float endAngle = angleTronquage / 2;

            for (float i = -90 + phi; i < 90 - phi; i += phi)
            {
                int t1 = points.Count;
                points.Add(new Vector3(0, Mathf.Sin(i * Mathf.PI / 180) * radius, 0));

                int t2 = points.Count;
                points.Add(new Vector3(0, Mathf.Sin((i + phi) * Mathf.PI / 180) * radius, 0));

                int t3 = points.Count;
                points.Add(new Vector3(
                    Mathf.Cos((startAngle * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius,
                    Mathf.Sin(i * Mathf.PI / 180) * radius,
                    Mathf.Sin((startAngle * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius));

                int t4 = points.Count;
                points.Add(new Vector3(
                    Mathf.Cos((startAngle * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius,
                    Mathf.Sin((i + phi) * Mathf.PI / 180) * radius,
                    Mathf.Sin((startAngle * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius));

                int t5 = points.Count;
                points.Add(new Vector3(
                    Mathf.Cos((endAngle * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius,
                    Mathf.Sin(i * Mathf.PI / 180) * radius,
                    Mathf.Sin((endAngle * Mathf.PI) / 180) * Mathf.Cos(i * Mathf.PI / 180) * radius));

                int t6 = points.Count;
                points.Add(new Vector3(
                    Mathf.Cos((endAngle * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius,
                    Mathf.Sin((i + phi) * Mathf.PI / 180) * radius,
                    Mathf.Sin((endAngle * Mathf.PI) / 180) * Mathf.Cos((i + phi) * Mathf.PI / 180) * radius));

                triangles.AddRange(new int[] { t1, t3, t2 });
                triangles.AddRange(new int[] { t4, t2, t3 });

                triangles.AddRange(new int[] { t1, t2, t5 });
                triangles.AddRange(new int[] { t6, t5, t2 });

                if (i == (-90 + phi))
                {
                    triangles.AddRange(new int[] { t1, indSouthPole, t3 });
                    triangles.AddRange(new int[] { t1, t5, indSouthPole });
                }

                //Positionnement triangle pole nord
                if (i >= 90 - (2 * phi))
                {
                    triangles.AddRange(new int[] { t2, t4, indNorthPole });
                    triangles.AddRange(new int[] { t2, indNorthPole, t6 });
                }
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = points.ToArray();
        mesh.uv = newUV;
        mesh.triangles = triangles.ToArray();
    }

    private void createCone()
    {
        int height = 5;
        float r = 1.0f;
        int resolution = 6;

        Mesh mesh = new Mesh();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = createVerticesCone(r, resolution, height); ;
        mesh.uv = newUV;
        mesh.triangles = createTrianglesCone(resolution);
    }

    private Vector3[] createVerticesCone(float r, int res, int h)
    {
        float pi = Mathf.PI;
        float x, y;
        List<Vector3> verts = new List<Vector3>();

        //Création du cercle du "bas"
        verts.Add(new Vector3(0, 0, 0));
        for (float teta = 0; teta <= 2 * pi; teta += (2 * pi) / res)
        {
            x = r * Mathf.Cos(teta);
            y = r * Mathf.Sin(teta);

            //Vérification nécessaire car superposition de points
            if (Mathf.Abs(x) < 0.00001) x = 0;
            if (Mathf.Abs(y) < 0.00001) y = 0;

            if (!verts.Contains(new Vector3(x, y, 0)))
            {
                verts.Add(new Vector3(x, y, 0));
            }
        }

        //Création du cercle du "haut"
        verts.Add(new Vector3(0, 0, h));

        //List<Vect3> -> Vec3[]
        newVertices = new Vector3[verts.Count];
        int compt = 0;
        foreach (Vector3 v3 in verts)
        {
            newVertices[compt] = v3;
            compt++;
        }

        return newVertices;
    }

    private int[] createTrianglesCone(int res)
    {
        List<int> ListTriangle = new List<int>();

        ListTriangle.Add(0);
        ListTriangle.Add(1);
        ListTriangle.Add(res);

        //For the down circle :
        for (int i = 0; i <= res - 2; i++)
        {
            ListTriangle.Add(0);
            ListTriangle.Add(i + 2);
            ListTriangle.Add(i + 1);
        }

        float n1, n2, n3;
        //res + 1 = h
        for (int i = 1; i < res; i++)
        {
            n1 = i;
            n2 = res + 1;
            n3 = i + 1;

            ListTriangle.Add(Convert.ToInt32(n1));
            ListTriangle.Add(Convert.ToInt32(n3));
            ListTriangle.Add(Convert.ToInt32(n2));
        }

        ListTriangle.Add(1);
        ListTriangle.Add(res + 1);
        ListTriangle.Add(res);

        //List<int> -> int[]
        newTriangles = new int[ListTriangle.Count];
        int compt = 0;
        foreach (int t in ListTriangle)
        {
            newTriangles[compt] = t;
            compt++;
        }

        return newTriangles;
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
            z = MathF.Sin(phi);

            vertices.Add(new Vector3(x, y, z));

            while(phi <= pi/2)
            {
                x = Mathf.Cos(teta) * Mathf.Cos(phi);
                y = MathF.Sin(teta) * Mathf.Cos(phi);
                z = Mathf.Sin(phi);

                vertices.Add(new Vector3(x, y, z));

                phi += interY;
            }
            teta += interX;
            phi = -pi / 2;
        }

        newVertices = new Vector3[vertices.Count];
        int compt = 0;
        foreach (Vector3 v3 in vertices)
        {
            newVertices[compt] = v3;
            compt++;
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

        newTriangles = new int[triangles.Count];
        int compt2 = 0;
        foreach (int t in triangles)
        {
            newTriangles[compt2] = t;
            compt2++;
        }

        //Create 
        Mesh mesh = new Mesh();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }

    private void CreateCylinder()
    {
        int height = 5;
        float r = 1.0f;
        int resolution = 6;

        Mesh mesh = new Mesh();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = createVerticesC(r, resolution, height); ;
        mesh.uv = newUV;
        mesh.triangles = createTrianglesC(resolution);
    }

    private Vector3[] createVerticesC(float r, int res, int h)
    {
        float pi = Mathf.PI;
        float x, y;
        List<Vector3> verts = new List<Vector3>();

        //Création du cercle du "bas"
        verts.Add(new Vector3(0, 0, 0));
        for (float teta = 0; teta <= 2 * pi; teta += (2 * pi) / res)
        {
            x = r * Mathf.Cos(teta);
            y = r * Mathf.Sin(teta);

            //Vérification nécessaire car superposition de points
            if (Mathf.Abs(x) < 0.00001) x = 0;
            if (Mathf.Abs(y) < 0.00001) y = 0;

            if (!verts.Contains(new Vector3(x, y, 0)))
            {
                verts.Add(new Vector3(x, y, 0));
                Debug.Log(x + " | " + y);
            }
        }

        //Création du cercle du "haut"
        verts.Add(new Vector3(0, 0, h));
        for (float teta = 0; teta <= 2 * pi; teta += (2 * pi) / res)
        {
            x = r * Mathf.Cos(teta);
            y = r * Mathf.Sin(teta);

            if (Mathf.Abs(x) < 0.00001) x = 0;
            if (Mathf.Abs(y) < 0.00001) y = 0;

            if (!verts.Contains(new Vector3(x, y, h)))
            {
                verts.Add(new Vector3(x, y, h));
            }
        }

        //List<Vect3> -> Vec3[]
        newVertices = new Vector3[verts.Count];
        int compt = 0;
        foreach (Vector3 v3 in verts)
        {
            newVertices[compt] = v3;
            compt++;
        }

        return newVertices;
    }

    private int[] createTrianglesC(int res)
    {
        List<int> ListTriangle = new List<int>();

        ListTriangle.Add(0);
        ListTriangle.Add(1);
        ListTriangle.Add(res);

        ListTriangle.Add(res + 1);
        ListTriangle.Add(res*2 + 1);
        ListTriangle.Add(res + 2);
        //For the up and down circle :
        for (int i = 0; i <= res - 2; i++)
        {
            ListTriangle.Add(0);
            ListTriangle.Add(i+2);
            ListTriangle.Add(i+1);

            ListTriangle.Add(res + 1);
            ListTriangle.Add(i + res + 2);
            ListTriangle.Add(i + res + 3);
        }

        float n1, n2, n3, n4;
        for (int i =1; i <= res - 1; i++)
        {
            n1 = i;
            n2 = i + res;
            n3 = i + res + 1;
            n4 = i + 1;

            if (n1 == res + 1 || n2 == res + 1 || n3 == res + 1 || n4 == res + 1)
            {
                ListTriangle.Add(Convert.ToInt32(n1));
                ListTriangle.Add(Convert.ToInt32(n4));
                ListTriangle.Add(Convert.ToInt32(n3));
            }
            else
            {
                ListTriangle.Add(Convert.ToInt32(n1));
                ListTriangle.Add(Convert.ToInt32(n3));
                ListTriangle.Add(Convert.ToInt32(n2));

                ListTriangle.Add(Convert.ToInt32(n1));
                ListTriangle.Add(Convert.ToInt32(n4));
                ListTriangle.Add(Convert.ToInt32(n3));
            }
            ListTriangle.Add(res);
            ListTriangle.Add(res*2 + 1);
            ListTriangle.Add(res*2);

            ListTriangle.Add(res);
            ListTriangle.Add(1);
            ListTriangle.Add(res*2 + 1);

            ListTriangle.Add(1);
            ListTriangle.Add(res + 2);
            ListTriangle.Add(res*2 + 1);
        }

        //List<int> -> int[]
        newTriangles = new int[ListTriangle.Count];
        int compt = 0;
        foreach (int t in ListTriangle)
        {
            newTriangles[compt] = t;
            compt++;
        }

        return newTriangles;
    }

    private void Create2x2Plane()
    {
        //Valeurs de x avec x >= 2
        int ligne = 4;
        int colone = 4;

        Mesh mesh = new Mesh();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = createVerticesP(ligne, colone);
        mesh.uv = newUV;
        mesh.triangles = createTrianglesP(mesh.vertices, colone, ligne);
    }

    private Vector3[] createVerticesP(int ligne, int colone)
    {
        List<Vector3> newVertices = new List<Vector3>();

        int compt = 0;
        for (int lig = 0; lig < ligne; lig++)
        {
            for (int col = 0; col < colone; col++)
            {
                newVertices.Add(new Vector3(lig, col, 0));
                compt++;
            }
        }

        //List vers tableau
        Vector3[] newVerticesT = new Vector3[newVertices.Count];
        int comptT = 0;
        foreach (Vector3 v3 in newVertices)
        {
            newVerticesT[comptT] = v3;
            comptT++;
        }

        return newVerticesT;
    }

    private int[] createTrianglesP(Vector3[] vertices, int ligne, int colone)
    {
        List<int> triangles = new List<int>();

        for (int Pl = 0; Pl < ligne - 1; Pl++)
        { 
            for (int Pc = 0; Pc < colone - 1; Pc++)
            {
                int point = Pc + (Pl * colone);
                triangles.Add(point);
                triangles.Add(point + 1);
                triangles.Add(point + colone + 1);

                triangles.Add(point);
                triangles.Add(point + colone + 1);
                triangles.Add(point + colone);
            }
        }

        //List vers tableau
        int[] newTrianglesT = new int[triangles.Count];
        int comptT = 0;
        foreach (int i in triangles)
        {
            newTrianglesT[comptT] = i;
            comptT++;
        }

        return newTrianglesT;
    }

}
