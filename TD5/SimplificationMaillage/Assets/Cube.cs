using System.Collections.Generic;
using UnityEngine;
using static Cube.Octree<int>;

public class Cube : MonoBehaviour
{
    public GameObject buddha;
    public Octree<int> bvh;

    private GameObject cube;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = buddha.transform.position;
        CreateOctree(position, 3.0f, 5);

        parcoursOctree(bvh.GetRoot(), position);    //Noeud racine, position, Rayon sphere (non interressant).
    }

    //Parcours de l'octree pour mettre en place la simplification de vertex
    private void parcoursOctree(OctreeNode<int> node, Vector3 center, int nodeDepth = 0)
    {
        if (!node.IsLeaf())
        {
            foreach (OctreeNode<int> subNode in node.Nodes)
            {
                parcoursOctree(subNode, center, nodeDepth + 1);
            }
        }
        else
        {
            evenVertexOctree(node);
        }
    }

    //Fonction qui vient verifier quel points est dans quel cube
    private void evenVertexOctree(OctreeNode<int> cube)
    {
        List<Vector3> verticesToEven = new List<Vector3>();
        float minBoundx = cube.Position.x - (cube.Size / 2);
        float minBoundy = cube.Position.y - (cube.Size / 2);
        float minBoundz = cube.Position.z - (cube.Size / 2);

        float maxBoundx = cube.Position.x + (cube.Size / 2); 
        float maxBoundy = cube.Position.y + (cube.Size / 2);
        float maxBoundz = cube.Position.z + (cube.Size / 2);

        //Parcours de tout nos vertex
        foreach (Vector3 vertex in buddha.GetComponent<MeshFilter>().mesh.vertices)
        {
            //Si vertex dans cube alors
            if (vertex.x > minBoundx && vertex.y > minBoundy && vertex.z > minBoundz)
            {
                if (vertex.x < maxBoundx && vertex.y < maxBoundy && vertex.z < maxBoundz)
                {
                    verticesToEven.Add(vertex);
                }
            }
        }
    }

    private void CreateOctree(Vector3 position, float taille, int res)
    {
        bvh = new Octree<int>(position, taille, res);
        //drawAllCubes(bvh.GetRoot(), position);
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