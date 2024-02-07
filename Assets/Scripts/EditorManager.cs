using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private List<Bar> bars;
    [SerializeField] private GameObject prefab_bar;
    [SerializeField] private GameObject prefab_note;
    private Vector2 mousePos;

    [SerializeField] private float bpm; // music bpm.
    [SerializeField] private int musicStartAfterBeats; // the number of initial beats. set 8.
    public bool isPause;

    [SerializeField] private int beatCnt; // counting beat.
    private float startTime; // start timing of audio system. 
    private float lastBeatTime; // timing of last beat.

    public float secondPerBeat; // second per beat. calculated by bpm.

    private int tailBarNum = 0;

    // note data.
    private static List<NoteData> noteList = new List<NoteData>();

    private 

    private float pauseTimer; // save the time when pause button is pressed.

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit rayHit;
            int layerMask = (1 << 6); // beat layer
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, layerMask))
            {
                Beat beat = rayHit.collider.gameObject.GetComponent<Beat>();
                GameObject obj = Instantiate(prefab_note);
            }
        }
        if (Input.GetMouseButton(0))
        {
            mainCamera.transform.position += new Vector3(0, (mousePos.y - Input.mousePosition.y) * 0.0001f, 0);
        }
    }

    public void MakeBar()
    {
        GameObject obj = Instantiate(prefab_bar, transform);
        Bar bar = obj.GetComponent<Bar>();
        bar.transform.position = new Vector3(0, tailBarNum, 0);
        bars.Add(bar);
        bar.num = bars.Count;
        tailBarNum += 8;
    }

    public void DeleteBar()
    {
        Destroy(bars[bars.Count - 1].gameObject);
        bars.RemoveAt(bars.Count - 1);
        tailBarNum -= 8;
    }

    public void MusicSelect()
    {

    }

    public void Pause() // when pause button is pressed in playing time.
    {
        isPause = true;
        audioManager.MusicPause();

        // record the time when pause button is pressed.
        pauseTimer = (float)AudioSettings.dspTime;

        // all of notes stop when pause button is pressed.
        for (int i = 0; i < transform.childCount; i++)
        {
            Note note = transform.GetChild(i).GetComponent<Note>();
            note.status = 0;
        }
    }

    public void PlayBack()
    {
        // unpause.
        isPause = false;

        // add the time passed during pause to lastBeatTime.
        lastBeatTime += (float)AudioSettings.dspTime - pauseTimer;
        audioManager.MusicPlay();

        // reactivate notes.
        int childCnt = transform.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            Note note = transform.GetChild(i).GetComponent<Note>();
            note.initialTime += (float)AudioSettings.dspTime - pauseTimer;
            note.status = 2;
        }
    }
}
