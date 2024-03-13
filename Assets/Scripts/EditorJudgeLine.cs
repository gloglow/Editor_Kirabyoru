using UnityEngine;

public class EditorJudgeLine : MonoBehaviour 
{
    // 判定線を描く

    // 判定線の形を決定する変数
    [SerializeField] private Vector2 idealScreenSize; // Resolution : 1920 x 1080.
    [SerializeField] private int lineRendererPosCnt; // 線を構成する点の数。 25で設定
    private Vector3 lineStartPos, lineEndPos; //　線が始まる点と終わる点
    [SerializeField] private float lineOffset; // 曲線の形を決定する変数。2で設定
    [SerializeField] private float lineYPos; // 線のy位置を決定。-3で設定

    private LineRenderer lineRenderer;
    [SerializeField] private CameraResolution cameraResolution; //　画面比率を同じにする

    [SerializeField] private GameObject prefab_Judge; // ノーツを認識するオブジェクト。
    [SerializeField] private float partWidth; // partの太さ。0.03で設定

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        SetLineStartEndPos();　//　判定線の始まる点と終わる点を決定
        DrawLine();　//　line rendererを使い、線を描く
        CreateLineParts();　//　partオブジェクトを線の上に配置
    }

    private void SetLineStartEndPos() // 判定線の始まる点と終わる点を決定
    {
        Vector2 lineScreenStartPos;　//　線が始まる画面上の座標
        float screenResolution = idealScreenSize.x / idealScreenSize.y;　//　理想的な画面比率

        if (!cameraResolution.isWideThanIdeal) // 横　＜　縦
        {
            lineScreenStartPos = new Vector2(0, Screen.height * 0.5f + Screen.width * (1 / screenResolution) * 0.5f);
        }
        else // 横　＞　縦
        {
            float tmp = Screen.height * screenResolution * 0.5f;
            lineScreenStartPos = new Vector2(Screen.width * 0.5f + tmp, Screen.height);
        }

        lineStartPos = Camera.main.ScreenToWorldPoint(lineScreenStartPos) + new Vector3(0, lineYPos, 0);
        lineEndPos = new Vector3(lineStartPos.x * (-1), lineStartPos.y);
        lineStartPos.z = 0;
        lineEndPos.z = 0;
    }

    private void DrawLine()
    {
        // 線を描くための変数
        Vector3 stPos, edPos, center;

        // line rendererを使い、線を描く
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
    }

    private void CreateLineParts()
    {
        // partオブジェクトの長さと大きさを初期化
        float length = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)); // 長さ：line rendererの点間距離
        prefab_Judge.transform.localScale = new Vector3(partWidth, length, partWidth);

        for (int i = 0; i < lineRendererPosCnt - 1; i++)　//　line rendererで描いた線に、ノーツを認識するオブジェクトを配置
        {
            GameObject judge = Instantiate(prefab_Judge, transform);

            Vector3 dirVec = lineRenderer.GetPosition(i+1) - lineRenderer.GetPosition(i);
            dirVec.Normalize();

            judge.transform.position = (lineRenderer.GetPosition(i) + lineRenderer.GetPosition(i+1)) / 2f;
            judge.transform.rotation = Quaternion.LookRotation(dirVec);
            judge.transform.Rotate(new Vector3(90, 0, 0));
        }
    }
}