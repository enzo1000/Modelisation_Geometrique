using UnityEngine;

public class HermiteCurve : MonoBehaviour
{
    [Header("Control Points")]
    public Vector3 point0;      // Point de contrôle initial
    public Vector3 tangent0;    // Tangente au point initial
    public Vector3 point1;      // Point de contrôle final
    public Vector3 tangent1;    // Tangente au point final

    [Header("Curve Settings")]
    [Range(2, 100)]
    public int resolution = 50; // Nombre de segments pour dessiner la courbe

    private void OnDrawGizmos()
    {
        // Dessine la courbe dans l'éditeur Unity
        Vector3 previousPoint = point0;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;    //On subdivise notre courbe en point de controle
            Vector3 currentPoint = CalculateHermitePoint(t, point0, tangent0, point1, tangent1);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(previousPoint, currentPoint);

            previousPoint = currentPoint;
        }

        // Dessine les points de contrôle
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(point0, 0.1f);
        Gizmos.DrawSphere(point1, 0.1f);
    }

    // Calcule un point sur la courbe d'Hermite.
    // t - Paramètre de la courbe (0 à 1)
    // p0 - Point de contrôle initial.
    // t0 - Tangente au point initial.
    // p1 - Point de contrôle final.
    // t1 - Tangente au point final.
    // retourne un point interpolé sur la courbe.
    private Vector3 CalculateHermitePoint(float t, Vector3 p0, Vector3 t0, Vector3 p1, Vector3 t1)
    {
        float h1 = 2 * t * t * t - 3 * t * t + 1;   // Coefficient pour p0
        float h2 = t * t * t - 2 * t * t + t;       // Coefficient pour t0
        float h3 = -2 * t * t * t + 3 * t * t;      // Coefficient pour p1
        float h4 = t * t * t - t * t;               // Coefficient pour t1
        return h1 * p0 + h2 * t0 + h3 * p1 + h4 * t1;
    }
}
