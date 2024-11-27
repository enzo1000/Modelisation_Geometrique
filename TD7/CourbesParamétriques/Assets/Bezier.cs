using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public GameObject[] points;

    [Header("Curve Settings")]
    [Range(2, 100)]
    public int resolution = 50;

    private int n;  //point.length - 1
    //u ou t = step entre pts
    //i = va de 0 a n (Somme)
    private List<Vector3> bezierPoint;

    private void Start()
    {
        bezierPoint = new List<Vector3>();
        n = transform.childCount - 1;

        for (float j = 0; j <= resolution; j++) { 
            float t = (j / resolution);

            Vector3 pointDeT = Vector3.zero;
            for (int i = 0; i <= n; i++)
            {
                pointDeT += polyBernstein(i, t) * points[i].transform.position;
            }
            bezierPoint.Add(pointDeT);
        }
    }

    private void OnDrawGizmos()
    {
        for (int j = 0; j < points.Length - 1; j++)
        {
            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawLine(points[j].transform.position, points[j + 1].transform.position);
        }
    }

    private void Update()
    {
        int i = 0;
        while (i < (bezierPoint.Count - 1))
        {
            Debug.Log(bezierPoint[i] + " index : " + i);
            Debug.DrawLine(bezierPoint[i], bezierPoint[i + 1], UnityEngine.Color.green);
            i++;
        }
    }

    float polyBernstein(int i, float t)
    {
        float res = factorial(n) / (factorial(i) * factorial(n - i));
        float res2 = res * Mathf.Pow(t, i);
        return res2 * Mathf.Pow((1 - t), (n - i));
    }

    int factorial(int chiffre)
    {
        int res = 1;

        for (int i = 0; i < chiffre; i++)
        {
            res *= chiffre - i;
        }

        return res;
    }
}
