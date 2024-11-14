using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cube.Octree<int>;

public class Cube : MonoBehaviour
{
    public GameObject buddha;
    public Octree<int> bvh;

    private GameObject cube;
    private List<Bounds> listCube = new();
    private Dictionary<int, int> associationVertices = new ();
    private List<Vector3> newVertices = new();
    private List<int> newTriangles = new();
    private List<Vector3> newNormals = new ();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 position = buddha.transform.position;
            CreateOctree(position, 3.0f, 4);

            //Peuple notre associationVertices et newVertices
            foreach (Bounds bo in listCube)
            {
                evenVertexOctree(bo);
            }

            //Change les indices de nos triangles
            int i = 0;
            int maxVal = 0; //Debug
            Mesh mesh = buddha.GetComponent<MeshFilter>().mesh;
            while (i < mesh.triangles.Length)
            {
                int t1 = associationVertices[mesh.triangles[i    ]];
                int t2 = associationVertices[mesh.triangles[i + 1]];
                int t3 = associationVertices[mesh.triangles[i + 2]];

                if (t1 != t2 && t2 != t3 && t3 != t1)
                {
                    newTriangles.Add(t1);
                    newTriangles.Add(t2);
                    newTriangles.Add(t3);
                }
                i += 3;
            }

            DestroyImmediate(buddha.GetComponent<MeshFilter>());
            buddha.AddComponent<MeshFilter>();
            Mesh newMesh = buddha.GetComponent<MeshFilter>().mesh;
            newMesh.vertices = newVertices.ToArray();
            newMesh.triangles = newTriangles.ToArray();
            newMesh.normals = TD2.processNormals(ref newVertices, ref newTriangles, ref newNormals);

            associationVertices.Clear();
            newVertices.Clear();
            listCube.Clear();
        }
    }

    private void evenVertexOctree(Bounds bounds)
    {
        //Liste qui vient stoquer les vertices a fusionner
        List<Vector3> verticesToEven = new List<Vector3>();

        //Stoque une nouvelle liste avec le nouveau points calcule (si calcule)
        //List<Vector3> newVertices = new List<Vector3>();

        //Parcours de tout nos vertex
        foreach (Vector3 vertex in buddha.GetComponent<MeshFilter>().mesh.vertices)
        {
            if (bounds.Contains(vertex))
            {
                if (verticesToEven.Count == 0)
                {
                    verticesToEven.Add(vertex);
                    newVertices.Add(vertex);
                    newNormals.Add(Vector3.zero);

                    //Debug
                    //GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    //obj.GetComponent<MeshRenderer>().material.color = Color.red;
                    //obj.transform.position = vertex;
                }
                //Cle = ancienne position, valeur = nouvelle position
                int index = Array.IndexOf(buddha.GetComponent<MeshFilter>().mesh.vertices, vertex);
                associationVertices.Add(index, newVertices.Count - 1);
            }
        }
    }

    private void CreateOctree(Vector3 position, float taille, int res)
    {
        bvh = new Octree<int>(position, taille, res);
        drawAllCubes(bvh.GetRoot(), position);
    }

    //nodeDepth utile pour l'appel recursif
    private void drawAllCubes(OctreeNode<int> node, Vector3 center, int nodeDepth = 0) 
    {
        //float distCube = Vector3.Distance(center, node.Position);

        //Si feuille
        if (!node.IsLeaf())
        {
            foreach (OctreeNode<int> subNode in node.Nodes)
            {
                drawAllCubes(subNode, center, nodeDepth + 1);
            }
        }
        else
        {
            CreateCube(node.Position, Vector3.one * node.Size);
        }
    }
    
    private void CreateCube(Vector3 center, Vector3 size)
    {
        Bounds newBounds = buddha.GetComponent<MeshRenderer>().bounds;
        newBounds.Expand(0.2f);
        if (newBounds.Contains(center)) {
            Bounds bounds = new Bounds(center, size);
            //cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.parent = GameObject.Find("BVH").transform;
            //cube.transform.position = center;
            //cube.transform.localScale = size;
            listCube.Add(bounds);
        }
    }

    //TType est un type generique
    public class Octree<TType>
    {
        private OctreeNode<TType> node; //List de node
        private Vector3 centerOctree;

        public Octree(Vector3 centre, float size, int depth)
        {
            centerOctree = centre;
            node = new OctreeNode<TType>(centre, size); //Notre octree a pour racine "node" qui est un OctreeNode
            node.Subdivision(depth);                    //On lance la subdivision de la racine vers les feuilles
        }

        public class OctreeNode<TType>
        {
            Vector3 position;
            float size;
            OctreeNode<TType>[] subNodes;

            //Une node correspond a une position et une taille (taille en float)
            public OctreeNode(Vector3 pos, float size)
            {
                position = pos;
                this.size = size;
            }

            //Getter de subNodes
            public IEnumerable<OctreeNode<TType>> Nodes
            {
                get { return subNodes; }
            }

            //Getter position
            public Vector3 Position
            {
                get { return position; }
            }

            //Getter size
            public float Size
            {
                get { return size; }
            }

            //Fonction servant a decouper notre Octree
            public void Subdivision(int depth)
            {
                //On creer notre nb de subNodes possible
                subNodes = new OctreeNode<TType>[8];

                for (int i = 0; i < subNodes.Length; i++)
                {
                    //Idee, pour positionner les cubes, on va devoir deplacer leurs centre :
                    // pour ca, on va utiliser les portes logiques et leurs valeurs binaire cf notes
                    Vector3 newPos = position;
                    if ((i & 4) == 4)
                    {
                        newPos.y += size * 0.25f;
                    }
                    else
                    {
                        newPos.y -= size * 0.25f;
                    }

                    if ((i & 2) == 2)
                    {
                        newPos.x += size * 0.25f;
                    }
                    else
                    {
                        newPos.x -= size * 0.25f;
                    }

                    if ((i & 1) == 1)
                    {
                        newPos.z += size * 0.25f;
                    }
                    else
                    {
                        newPos.z -= size * 0.25f;
                    }

                    //On vient creer une nouvelle node a la position vu plus haut et avec une taille divise par 2
                    subNodes[i] = new OctreeNode<TType>(newPos, size * 0.5f);

                    //Cas specifique de profondeur non atteinte :
                    if (depth > 0)
                    {
                        //Appel recursif
                        subNodes[i].Subdivision(depth - 1);
                    }
                }
            }

            //Getter inline pour verifier la node visee est une feuilles de l'arbre
            public bool IsLeaf() { return subNodes == null; }
        }

        public Vector3 GetCenterOctree() { return centerOctree; }

        //Getter pour acceder a la racine de notre arbre
        public OctreeNode<TType> GetRoot() { return node; }
    }
}