using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Cube.Octree<int>;

public class Cube : MonoBehaviour
{
    public Vector3[] coordSphere;

    private GameObject cube;
    private GameObject sphere;

    // Start is called before the first frame update
    void Start()
    {
        sphere = GameObject.FindGameObjectsWithTag("Sphere")[0];

        //Pour l'instant on créer 1 sphere et on la reutilise,
        // donc on passe different parametres de position mais une 
        // seule sphere pour ses informations tel que son rayon ...
        //CreateOctree(3, new Vector3(0, 0, 0), sphere);
        foreach (Vector3 coord in coordSphere)
        {
            CreateOctree(5, coord, sphere);
        }

        //Pour l'intersection
        Octree<int> octree1 = new Octree<int>(sphere, new Vector3(0, 0, 0), 2.0f, 3);
        Octree<int> octree3 = new Octree<int>(sphere, new Vector3(-1, -1, -1), 2.0f, 3);

        //On vient merge tout nos cubes pour calculer l'intersection
        OctreeNode<int>[] nodes = mergeOctrees(octree1, octree3);

        //Pour chaque cube merge
        foreach(var node in nodes)
        {
            intersectionSphere(octree1, octree3, node);
        }
    }

    //Merge les nodes de deux octree donne en parametre pour realiser des operation d'intersection ou autre 
    private OctreeNode<int>[] mergeOctrees(Octree<int> oc1, Octree<int> oc2)
    {
        List<OctreeNode<int>> mergeList1 = new();
        List<OctreeNode<int>> mergeList2 = new();

        //On commence a la racine
        mergeNodes(oc1.GetRoot(), ref mergeList1);
        mergeNodes(oc2.GetRoot(), ref mergeList2);

        //Merge de mergeList 2 dans mergeList 1
        mergeList1.AddRange(mergeList2);

        //renvoi de la liste commune
        return mergeList1.ToArray();
    }

    //Le gros probleme avec cette fonction c'est que le return vient casser tout le parcours
    // de notre arbre. 1er pb, on ne parcours pas toutes les feuilles de l'arbre
    // 2eme pb, on ne parcours pas toutes les feuilles de la branches n-1
    // Edit : Pb regle, j'etais mal alune la veille car la solution est au final tres "simple" a mettre en place
    private void mergeNodes(OctreeNode<int> node, ref List<OctreeNode<int>> mergeList)
    {
        //Si node != feuille alors on parcours en profondeur notre octree
        if (!node.IsLeaf())
        {
            foreach(var subNode in node.Nodes)
            {
                mergeNodes(subNode, ref mergeList);
            }
        }
        //Si cas node == feuille
        else
        {
            mergeList.Add(node);
        }
    }

    private void CreateCube(Vector3 center, Vector3 size)
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = center;
        cube.transform.localScale = size;
    }

    private void CreateOctree(int res, Vector3 position, GameObject sphere)
    {
        //On vient generer notre octree
        Octree<int> octree = new Octree<int>(sphere, position, 2.0f, res);
        drawAllCubes(octree.GetRoot(), position, sphere.GetComponent<Sphere>().getRayon());
    }

    private void intersectionSphere(Octree<int> oc1, Octree<int> oc2, OctreeNode<int> node)
    {
        float distCube1 = Vector3.Distance(oc1.GetCenterOctree(), node.Position);
        float distCube2 = Vector3.Distance(oc2.GetCenterOctree(), node.Position);

        if (distCube1 <= oc1.GetRayonOctree() && distCube2 <= oc2.GetRayonOctree())
        {
            CreateCube(node.Position, Vector3.one * node.Size);
        }
    }

    //nodeDepth utile pour l'appel recursif
    private void drawAllCubes(OctreeNode<int> node, Vector3 center, float rayon, int nodeDepth = 0) 
    {
        float distCube = Vector3.Distance(center, node.Position);

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
            //Si, la node.Position dans le rayon de notre sphère alors : generer cube
            if (distCube < rayon)   
            {
                CreateCube(node.Position, Vector3.one * node.Size); //Convertion de la size en Vector3
            }
        }
    }

    //TType est un type generique
    public class Octree<TType>
    {
        private OctreeNode<TType> node; //List de node
        private float rayonOctree;
        private Vector3 centerOctree;

        public Octree(GameObject sphere, Vector3 centre, float size, int depth)
        {
            centerOctree = centre;
            rayonOctree = sphere.GetComponent<Sphere>().getRayon();
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

        public float GetRayonOctree() { return rayonOctree; }

        //Getter pour acceder a la racine de notre arbre
        public OctreeNode<TType> GetRoot() { return node; }
    }
}