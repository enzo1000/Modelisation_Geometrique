using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimplificationCourbes : MonoBehaviour
{
    public List<Vector3> spheres;

    [Range(0.0f, 10.0f)]
    public int resolution = 10;

    private void OnDrawGizmos()
    {
        //Affiche les pts originaux
        Gizmos.color = Color.yellow;
        for (int i = 0; i < spheres.Count - 1; i++)
        {
            Gizmos.DrawLine(spheres[i], spheres[i+1]);
        }
        Gizmos.DrawLine(spheres[^1], spheres[0]);
    }

    private void OnDrawGizmosSelected()
    {
        //Subdivision des pts quand selected seulement
        List<Vector3> newPoints = spheres;

        for (int i = 0; i < resolution; i++)
        {
            List<Vector3> temp = new List<Vector3>();
            for (int j = 0; j < newPoints.Count - 1; j++)
            {
                Vector3 p0 = newPoints[j];
                Vector3 p1 = newPoints[j+1];
                Vector3 q0 = p0 + (p1 - p0) * 0.25f;
                Vector3 q1 = p0 + (p1 - p0) * 0.75f;

                temp.Add(q0);
                temp.Add(q1);
            }
            Vector3 p00 = newPoints[newPoints.Count - 1];
            Vector3 p11 = newPoints[0];
            Vector3 q00 = p00 + (p11 - p00) * 0.25f;
            Vector3 q11 = p00 + (p11 - p00) * 0.75f;

            temp.Add(q00);
            temp.Add(q11);
            newPoints = temp;
        }

        //Affiche les nouveux pts (dependant de la res)
        Gizmos.color = Color.blue;
        for (int i = 0; i < newPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
        }
        Gizmos.DrawLine(newPoints[newPoints.Count - 1], newPoints[0]);
        Debug.Log(newPoints.Count);
    }
}
