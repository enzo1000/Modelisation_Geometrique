using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

public class TD2 : MonoBehaviour
{
    public GameObject buddha;
    public Material buddhaMat;

    void Awake()
    {
        //Liste de nos points et triangles
        List<Vector3> vertices_ = new List<Vector3>();
        List<int> triangles_ = new List<int>();
        List<Vector3> normals_ = new List<Vector3>();

        //Init mesh
        Mesh mesh = new Mesh();

        //Pour calculer le centre de gravité
        Vector3 centreGravite = Vector3.zero;

        //Nb de ligne concernant la partie Vertex du fichier
        int nbLignePoint = 0;

        buddha = gameObject;
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<Renderer>().material = buddhaMat;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        string Path = @"../buddha.off";


        //Si le fichier existe
        if (File.Exists(Path))
        {
            Debug.Log("Le fichier buddha.off a été localisé");

            //Compteur pour récuperer l'indice de la ligne courante
            int indexLigne = 0;
            
            //Découpage du fichier en ligne
            foreach (string line in File.ReadLines(Path))
            {
                //On ignore la première ligne
                if (indexLigne != 0)
                {
                    string[] tabL = line.Split(' ');    //Découpe des lignes en 3 parties

                    //Ligne 1 informations taille du fichier
                    if (indexLigne == 1)
                    {
                        nbLignePoint = Int32.Parse(tabL[0]);    //String vers int
                    }
                    else if (indexLigne < nbLignePoint + 2)    //Offset de 2 sur les lignes
                    {
                        //Permet de convertir une ligne de string en un vecteur3 de float(s)
                        float p1 = float.Parse(tabL[0], CultureInfo.InvariantCulture.NumberFormat);
                        float p2 = float.Parse(tabL[1], CultureInfo.InvariantCulture.NumberFormat);
                        float p3 = float.Parse(tabL[2], CultureInfo.InvariantCulture.NumberFormat);

                        Vector3 newVec = new Vector3(p1, p2, p3);

                        centreGravite += newVec;

                        vertices_.Add(newVec);
                        normals_.Add(Vector3.zero);
                    }
                    else
                    {
                        int t1 = Int32.Parse(tabL[1]);
                        int t2 = Int32.Parse(tabL[2]);
                        int t3 = Int32.Parse(tabL[3]);

                        triangles_.Add(t1);
                        triangles_.Add(t2);
                        triangles_.Add(t3);
                    }
                }
                indexLigne++; 
            }
        }
        else
        {
            Debug.Log(Path);
        }

        processGravityCenter(centreGravite, ref vertices_);
        processNormaliseScale(ref vertices_);
        
        mesh.vertices = vertices_.ToArray();
        mesh.triangles = triangles_.ToArray();
        mesh.normals = processNormals(ref vertices_, ref triangles_, ref normals_);

        exportObj(ref vertices_, ref triangles_, ref normals_);
    }

    void processGravityCenter(Vector3 centerGrav, ref List<Vector3> vertices_)
    {
        //Partie processing centre de gravité
        centerGrav = centerGrav / (float)vertices_.Count;

        for (int i = 0; i < vertices_.Count; i++)
        {
            vertices_[i] += (Vector3.zero - centerGrav);
        }
    }
    void processNormaliseScale(ref List<Vector3> vertices_)
    {
        float maxX = 0;
        float maxY = 0;
        float maxZ = 0;

        //Récupération des max sur chaque axe
        foreach (Vector3 v in vertices_)
        {
            if (Mathf.Abs(v.x) > maxX)
            {
                maxX = Mathf.Abs(v.x);
            }
            if (Mathf.Abs(v.y) > maxY)
            {
                maxY = Mathf.Abs(v.y);
            }
            if (Mathf.Abs(v.z) > maxZ)
            {
                maxZ = Mathf.Abs(v.z);
            }
        }

        //Division par le max et remplacement des vertices
        for (int i = 0; i < vertices_.Count; i++)
        {
            float maxM = Mathf.Max(maxX, maxY, maxZ);
            Vector3 tmp = vertices_[i] / maxM;
            vertices_ [i] = tmp;
        }
    }
    Vector3[] processNormals(ref List<Vector3> vertices_, ref List<int> triangles_, ref List<Vector3> normals_)
    {
        //Parcours des triangles pour avoir la liste des points connexes
        for (int i = 0; i < triangles_.Count; i += 3)
        {
            //Une normale = produit vectoriel des deux premières arêtes
            //b - a et c - a
            int ia = triangles_[i];
            int ib = triangles_[i + 1];
            int ic = triangles_[i + 2];

            Vector3 ab = vertices_[ib] - vertices_[ia];
            Vector3 ac = vertices_[ic] - vertices_[ia];

            Vector3 norm = Vector3.Cross(ab, ac);

            normals_[ia] += norm;
            normals_[ib] += norm;
            normals_[ic] += norm;
        }

        for (int i = 0; i < normals_.Count; i ++)
        {
            normals_[i].Normalize();
        }

        return normals_.ToArray();
    }
    void exportObj(ref List<Vector3> vertices, ref List<int> triangle, ref List<Vector3> normals)
    {
        string Path = @"../buddha.obj";

        if (File.Exists(Path))
        {
            //Debug.Log("Suppression buddha");
            File.Delete(Path);
        }

        //Initialise le stream writer vers le fichier cible (Créer le fichier)
        using (StreamWriter sw = new StreamWriter(Path))
        {
            //Debug.Log("Init buddha a : " + Path);

            //Format purpose
            sw.WriteLine("");
            sw.WriteLine("o buddha");
            sw.WriteLine("");

            foreach (Vector3 v in vertices) 
            { 
                sw.WriteLine("v " + v.x.ToString(CultureInfo.InvariantCulture) + " " + v.y.ToString(CultureInfo.InvariantCulture) + " " + v.z.ToString(CultureInfo.InvariantCulture));
            }

            sw.WriteLine("");

            foreach (Vector3 n in normals) 
            { 
                sw.WriteLine("vn " + n.x.ToString(CultureInfo.InvariantCulture) + " " + n.y.ToString(CultureInfo.InvariantCulture) + " " + n.z.ToString(CultureInfo.InvariantCulture));
            }

            sw.WriteLine("");

            for (int i = 0; i < triangle.Count; i += 3)
            {
                int p1 = triangle[i] + 1;
                int p2 = triangle[i + 1] + 1;
                int p3 = triangle[i + 2] + 1;

                sw.WriteLine("f " + p1 + " " + p2 + " " + p3);
            }
        }
    }
}
