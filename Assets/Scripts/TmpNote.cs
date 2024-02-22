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
    
    public int status;

    private Vector3 startPos;
    [SerializeField] private float spawnYPos;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        switch (status)
        {
            case 0: // initialize
                Initialize();
                break;
            case 1: // decide note xPos
                DecideXPos();
                break;
            case 2: // decide direction vector
                DecideDirection();
                break;
        }
    }

    private void Initialize()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        status = 1;
    }

    private void DecideXPos()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            transform.position = new Vector3(mouseX, spawnYPos, 0);
        }
        if (Input.GetMouseButtonUp(0))
        {
            xPos = transform.position.x;
            startPos = transform.position;
            status = 2;
        }
    }

    private void DecideDirection()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            dirVec = mousePos - transform.position;
            dirVec.Normalize();

            int layerMask = (1 << 7); // layer of judgeline
            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, dirVec, out rayHit, Mathf.Infinity, layerMask))
            {
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, rayHit.collider.transform.position);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            editorManager.AddNote(xPos, bar, beat, dirVec.x, dirVec.y);

            gameObject.SetActive(false);
        }
    }
}
