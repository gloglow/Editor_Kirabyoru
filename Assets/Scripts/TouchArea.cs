using UnityEngine;

public class TouchArea : MonoBehaviour
{
    [SerializeField] private EditorJudgeLine judgeLine;
    private LineRenderer lineRenderer;
    private MeshCollider meshCollider; // to sense input(mouse or touch)

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Draw()
    {
        Vector3[] points = judgeLine.GetLinePoints();
        // draw line with points.
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }

        // make mesh collider to sense input(ray from mouse or touch)
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
    }
}
