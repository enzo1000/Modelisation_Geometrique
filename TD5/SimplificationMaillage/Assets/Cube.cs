using System.Collections.Generic;
using UnityEngine;
using static Cube.Octree<int>;

public class Cube : MonoBehaviour
{
    public GameObject buddha;
    public Octree<int> bvh;

    private GameObject cube;
    private List<GameObject> listCube = new List<GameObject>();
    private List<Vector3> listeVertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = buddha.transform.position;
        CreateOctree(position, 2.0f, 3);

        foreach(GameObject go in listCube)
        {
            evenVertexOctree(go);
        }

        foreach(var v in listeVertices)
        {
            Debug.Log(v);
        }
    }

    private void evenVertexOctree(GameObject cube)
    {
        //Liste qui vient stoquer les vertices a fusionner
        List<Vector3> verticesToEven = new List<Vector3>();
        //Stoque une nouvelle liste avec le nouveau points calcule (si calcule)
        List<Vector3> newVertices = new List<Vector3>();

        //Parcours de tout nos vertex
        foreach (Vector3 vertex in buddha.GetComponent<MeshFilter>().mesh.vertices)
        {
            if (cube.GetComponent<MeshRenderer>().bounds.Contains(vertex))
            {
                verticesToEven.Add(vertex);
            }
        }

        if (verticesToEven.Count > 0)
        {
            listeVertices.Add(verticesToEven[0]);
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
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = GameObject.Find("BVH").transform;
        cube.transform.position = center;
        cube.transform.localScale = size;
        listCube.Add(cube);
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