using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpNote : MonoBehaviour
{
    float xPos;
    public int barNum;
    public float beatNum;
    public Vector3 dirVec;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private EditorManager editorManager;
    [SerializeField] private GameObject prefab_editorNote;
    Camera cam;
    public int status;

    Vector3 startPos;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        switch (status)
        {
            case 0: // initialize
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.zero);
                status = 1;
                break;
            case 1:
                if (Input.GetMouseButton(0))
                {
                    float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                    transform.position = new Vector3(mouseX, 4, 0);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    xPos = transform.position.x;
                    startPos = transform.position;
                    status = 2;
                }
                break;
            case 2:
                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos = new Vector3(mousePos.x, mousePos.y, 0);
                    dirVec = mousePos - transform.position;
                    dirVec.Normalize();
                    int layerMask = (1 << 7);
                    RaycastHit rayHit;
                    if (Physics.Raycast(transform.position, dirVec, out rayHit, Mathf.Infinity, layerMask))
                    {
                        lineRenderer.SetPosition(0, startPos);
                        lineRenderer.SetPosition(1, rayHit.collider.transform.position);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    GameObject obj = Instantiate(prefab_editorNote);
                    EditorNote note = obj.GetComponent<EditorNote>();
                    note.xPos = xPos;
                    note.bar = barNum;
                    note.beat = beatNum;
                    note.unitVecX = dirVec.x;
                    note.unitVecY = dirVec.y;
                    editorManager.AddNote(note);
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
