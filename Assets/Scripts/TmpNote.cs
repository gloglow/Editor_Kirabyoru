using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpNote : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private EditorManager editorManager;

    public float xPos;
    public int bar;
    public float beat;
    public Vector3 dirVec;
    
    public int status;　//　動作を制御する変数

    private Vector3 startPos;
    [SerializeField] private float spawnYPos;

    public bool flagMake;　//　true：ノーツを作る　false：ノーツの方向ベクトルだけ修正

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        switch (status)
        {
            case 0: // 初期化
                Initialize();
                break;
            case 1: // ノーツのx座標を決定
                DecideXPos();
                break;
            case 2: // ノーツの方向ベクトルを決定
                DecideDirection();
                break;
        }
    }

    private void Initialize()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);

        if (flagMake)
            status = 1;
        else
            status = 2;
    }

    private void DecideXPos()　//　ノーツのx座標を決定
    {
        if (Input.GetMouseButton(0))　//　マウスでドラッグし、位置を決定
        {
            float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            mouseX = Mathf.RoundToInt(mouseX);
            transform.position = new Vector3(mouseX, spawnYPos, 0);
        }
        if (Input.GetMouseButtonUp(0))
        {
            xPos = transform.position.x;
            status = 2;
        }
    }

    private void DecideDirection()　// ノーツの方向ベクトルを決定
    {
        startPos = transform.position;
        if (Input.GetMouseButton(0))　//　マウスでドラッグし、方向を決定
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            dirVec = mousePos - transform.position;
            dirVec.Normalize();
            dirVec = new Vector3(OnlyTenthPlaceValue(dirVec.x), OnlyTenthPlaceValue(dirVec.y), 0);

            int layerMask = (1 << 7); // 判定線のpartオブジェクトのレイヤー
            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, dirVec, out rayHit, Mathf.Infinity, layerMask))
            {
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, rayHit.collider.transform.position);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (flagMake)
                editorManager.AddNote(xPos, bar, beat, dirVec.x, dirVec.y);
            else
                editorManager.ModifyNoteDir(dirVec.x, dirVec.y);
            editorManager.status = 2;
            gameObject.SetActive(false);
        }
    }
    
    private float OnlyTenthPlaceValue(float f)
    {
        return Mathf.Floor(f * 10f) / 10f;
    }
}
