using UnityEngine;

public class EditorJudgeLine : MonoBehaviour 
{
    // draw judge line and destroy line.

    // values for draw line
    [SerializeField] private Vector2 idealScreenSize; // Resolution Reference : 1920 x 1080.
    [SerializeField] private int lineRendererPosCnt;
    public  Vector3 lineStartPos, lineEndPos;
    [SerializeField] private float lineOffset;

    private LineRenderer lineRenderer;
    public CameraResolution cameraResolution;

    [SerializeField] private GameObject part; // object what actually check note
    [SerializeField] private float partWidth; // width of object. best is 0.03.

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        ReadyToDrawLine();
        GetLinePoint();
    }

    private void ReadyToDrawLine()
    {
        Vector2 lineScreenStartPos;
        float screenResolution = 16f / 9f;
        float lineHeightPos = 1f;
        if (cameraResolution.scaleHeight < 1) // wide height
        {
            lineScreenStartPos = new Vector2(0, Screen.height * 0.5f + Screen.width * (1 / screenResolution) * 0.5f * lineHeightPos);
        }
        else // wide width
        {
            float tmp = Screen.height * screenResolution * 0.5f;
            lineScreenStartPos = new Vector2(Screen.width * 0.5f + tmp, Screen.height * lineHeightPos);
        }
        // To make ideal line for every resolution
        lineStartPos = Camera.main.ScreenToWorldPoint(lineScreenStartPos) + new Vector3(0, -2, 0);
        lineEndPos = new Vector3(lineStartPos.x * (-1), lineStartPos.y) + new Vector3(0, -2, 0);
        lineStartPos.z = 0;
        lineEndPos.z = 0;
    }

    private void GetLinePoint()
    {
        // variables for drawing line
        Vector3 stPos, edPos, center;

        // draw parabola with linerenderer and Slerp. (visual)
        for (int i = 0; i < lineRendererPosCnt; i++)
        {
            stPos = lineStartPos;
            edPos = lineEndPos;
            center = (stPos + edPos) * 0.5f;
            center.y += lineOffset;
            stPos = stPos - center;
            edPos = edPos - center;
            Vector3 point = Vector3.Slerp(stPos, edPos, i / (float)(lineRendererPosCnt - 1));
            point += center;
            lineRenderer.SetPosition(i, point);
        }

        Draw(); // draw real judge line and destroy line.
    }

    public void Draw()
    {
        // initialize size of part object.
        float length = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)); // length = distance between two points of line renderer.
        part.transform.localScale = new Vector3(partWidth, length, partWidth);

        for (int i = 0; i < lineRendererPosCnt - 1; i++)
        {
            // instantiate part object and initialize their position and rotation.
            GameObject judgePart = Instantiate(part, transform);

            Vector3 dirVec = lineRenderer.GetPosition(i+1) - lineRenderer.GetPosition(i);
            dirVec.Normalize();

            judgePart.transform.position = (lineRenderer.GetPosition(i) + lineRenderer.GetPosition(i+1)) / 2f;
            judgePart.transform.rotation = Quaternion.LookRotation(dirVec);
            judgePart.transform.Rotate(new Vector3(90, 0, 0));
        }
    }

    public Vector3[] GetLinePoints() // get line points for draw toucharea.
    {
        Vector3[] arr = new Vector3[lineRendererPosCnt];
        int tmp = lineRenderer.GetPositions(arr);
        return arr;
    }
}
