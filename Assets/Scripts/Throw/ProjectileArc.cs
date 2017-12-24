using UnityEngine;

public class ProjectileArc : MonoBehaviour
{
    [SerializeField]
    int iterations = 20;

    [SerializeField]
    Color errorColor;

    private Color initialColor;
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer.material.HasProperty("_Color"))
            initialColor = lineRenderer.material.color;
        else
            initialColor = lineRenderer.material.GetColor("_TintColor");
    }

    public void UpdateArc(float speed, float distance, float gravity, float angle, Vector3 direction, bool valid)
    {
        Vector2[] arcPoints = ProjectileMath.ProjectileArcPoints(iterations, speed, distance, gravity, angle);
        Vector3[] points3d = new Vector3[arcPoints.Length];

        for (int i = 0; i < arcPoints.Length; i++)
        {
            points3d[i] = new Vector3(0, arcPoints[i].y, arcPoints[i].x);
        }

        lineRenderer.positionCount = arcPoints.Length;
        lineRenderer.SetPositions(points3d);

        transform.rotation = Quaternion.LookRotation(direction);

        if (lineRenderer.material.HasProperty("_Color"))
            lineRenderer.material.color = valid ? initialColor : errorColor;
        else
            lineRenderer.material.SetColor("_TintColor", valid ? initialColor : errorColor);
    }
}
