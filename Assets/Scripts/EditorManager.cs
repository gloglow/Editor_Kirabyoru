using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Jobs;

public class EditorManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ObjectPoolManager poolManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private List<Bar> barList;
    [SerializeField] private GameObject prefab_bar;
    [SerializeField] private GameObject prefab_note;
    [SerializeField] private TmpNote tmpNote;
    [SerializeField] private GameObject bars;
    private Vector2 mousePos;

    public float bpm; // music bpm.
    [SerializeField] private int musicStartAfterBeats; // the number of initial beats. set 8.
    public bool isPause;

    [SerializeField] private int beatCnt; // counting beat.
    private float startTime; // start timing of audio system. 
    private float lastBeatTime; // timing of last beat.
    int index;

    public float secondPerBeat; // second per beat. calculated by bpm.

    private int tailBarNum = 4;

    // note data.
    public List<EditorNote> noteList = new List<EditorNote>();
    public EditorNote targetNote;
    public int targetNoteIndex;

    private float pauseTimer; // save the time when pause button is pressed.
    private bool flagMakeNote = false;

    private float tmpCameraPos = 0;

    private bool flagPlay = false;

    public int crtIndex;
    public float spawnYPos;

    private void Start()
    {
        startTime = (float)AudioSettings.dspTime;
        lastBeatTime = startTime;
        isPause = true;
        index = 0;
    }
    void Update()
    {
        if(!flagMakeNote && !flagPlay)
        {
            ScreenDrag();
            if ((Input.GetMouseButtonUp(0))
                && (EventSystem.current.IsPointerOverGameObject() == false))
            {
                if (Vector3.Distance(mousePos, Input.mousePosition) < 1)
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);
                    RaycastHit rayHit;
                    int layerMask = (1 << 6);
                    if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
                    {
                        int rayHitLayer = rayHit.transform.gameObject.layer;
                        if(rayHitLayer == 6)
                        {
                            bars.gameObject.SetActive(false);
                            uiManager.NoteMakeMode();
                            tmpCameraPos = Camera.main.transform.position.y;
                            Camera.main.transform.position = new Vector3(0, 0, -10);

                            SubBeat beatPart = rayHit.transform.gameObject.GetComponent<SubBeat>();
                            tmpNote.gameObject.SetActive(true);
                            tmpNote.barNum = beatPart.barNum;
                            tmpNote.beatNum = beatPart.beatNum;
                            tmpNote.transform.position = new Vector3(0, 4, 0);
                            tmpNote.status = 0;
                            flagMakeNote = true;
                        }
                        else if (rayHitLayer == 8)
                        {
                            GameObject obj = rayHit.transform.gameObject;
                            targetNoteIndex = FindNoteIndex(obj.GetComponent<EditorNote>());
                            targetNote = noteList[targetNoteIndex];
                            uiManager.ShowNoteInfo(new Vector2(targetNote.bar, targetNote.beat), 
                                targetNote.xPos, targetNote.unitVecX, targetNote.unitVecY);
                        }
                        
                    }
                                       
                }
            }
        }
        else if (flagPlay)
        {
            // count beat because note should be activated and move with beat timing.
            if (AudioSettings.dspTime - lastBeatTime > secondPerBeat) // if over one beat time passed from last beat time, count beat.
            {
                beatCnt++;
                lastBeatTime += secondPerBeat;

                // after first 8 beats, music start.
                if (beatCnt == musicStartAfterBeats + 1)
                {
                    audioManager.MusicPlay();
                }

                // there is any note data && current beat == note beat, make note.
                while (noteList.Count > index && (beatCnt - musicStartAfterBeats + 2) == (int)noteList[index].beat)
                {
                    float f; // the waiting time of note. (if note is 1/4 note, not wait.)
                    if (noteList[index].beat == (beatCnt - musicStartAfterBeats + 1)) // 1/4 note.
                    {
                        f = 0;
                    }
                    else // 1/8 note, 1/16 note, etc
                    {
                        // if beat of a note is 6.5, at 6th beat, wait 0.5beat time and be made.
                        f = (noteList[index].beat - (beatCnt - musicStartAfterBeats + 1)) * secondPerBeat * 0.5f;
                    }
                    StartCoroutine(MakeNote(f));
                    index++;
                }
            }
        }
    }

    IEnumerator MakeNote(float time)
    {
        yield return new WaitForSeconds(time);
        // bring note from note object pool.
        GameObject obj = poolManager.notePool.Get();
        Note note = obj.GetComponent<Note>();
        note.transform.parent = transform;
        note.editorManager = this;

        // activate note.
        note.status = 1;
        note.transform.position = new Vector3(noteList[crtIndex].xPos, spawnYPos, 0);
        note.dirVec = new Vector3(noteList[crtIndex].unitVecX, noteList[crtIndex].unitVecY, 0);
        crtIndex++;
        if (isPause) note.status = 0;
    }

    public void MusicPlay()
    {
        if (audioManager.audioSource == null)
            return;
        Camera.main.transform.position = new Vector3(0, -2, -10);
        flagPlay = true;
        crtIndex = 0;
        index = 0;
        beatCnt = 0;
        lastBeatTime = (float)AudioSettings.dspTime;
        isPause = false;
        bars.gameObject.SetActive(false);
    }

    public void MusicStop()
    {
        flagPlay = false;
        isPause = true;
        audioManager.MusicStop();
        bars.gameObject.SetActive(true);
    }

    private int FindNoteIndex(EditorNote note)
    {
        for(int i = 0; i<noteList.Count; i++)
        {
            if (noteList[i] == note)
                return i;
        }
        return -1;
    }

    private float floatTruncate(float n, int power)
    {
        return Mathf.Floor(n * Mathf.Pow(10, power)) / Mathf.Pow(10, power);
    }

    public void Initialize()
    {
        while(barList.Count > 0)
        {
            DeleteBar();
        }
    }

    private void ScreenDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            mainCamera.transform.position += new Vector3(0, (mousePos.y - Input.mousePosition.y) * 0.0001f, 0);
        }
    }

    public void MakeBar()
    {
        GameObject obj = Instantiate(prefab_bar, bars.transform);
        Bar bar = obj.GetComponent<Bar>();
        bar.barNum = barList.Count + 1;
        bar.transform.position = new Vector3(0, tailBarNum, 0);
        barList.Add(bar);
        tailBarNum += 32;
    }

    public void DeleteBar()
    {
        if (barList.Count <= 0)
            return;
        int i;
        for (i =0; i < noteList.Count; i++)
        {
            if (noteList[i].bar >= barList.Count)
            {
                noteList.RemoveRange(i, noteList.Count - i);
                break;   
            }
        }
        Destroy(barList[barList.Count - 1].gameObject);
        barList.RemoveAt(barList.Count - 1);
        tailBarNum -= 32;
    }

    public void AddNote(EditorNote editorNote)
    {
        int actualBar = editorNote.bar - 1;
        float actualBeat = editorNote.beat - 1;
        while (barList.Count <= actualBar)
        {
            MakeBar();
        }
        editorNote.transform.parent = barList[actualBar].transform.GetChild((int)actualBeat).transform.GetChild((int)((actualBeat - (int)actualBeat) / 0.25f));
        editorNote.transform.position = new Vector3(editorNote.xPos, 0, 0);
        editorNote.transform.localPosition = new Vector3(editorNote.transform.localPosition.x, 0, 0);
        
        noteList.Add(editorNote);
        noteList.Sort(NoteCompare);
        flagMakeNote = false;
        bars.gameObject.SetActive(true);
        uiManager.BackToDefault();
        Camera.main.transform.position = new Vector3(0, tmpCameraPos, -10);
    }

    public void AddNote(float xPos, int bar, float beat, float unitVecX, float unitVecY)
    {
        GameObject obj = Instantiate(prefab_note);
        EditorNote editorNote = obj.GetComponent<EditorNote>();
        editorNote.xPos = xPos;
        editorNote.bar = bar;
        editorNote.beat = beat;
        editorNote.unitVecX = unitVecX;
        editorNote.unitVecY = unitVecY;
        AddNote(editorNote);
    }

    private int NoteCompare(EditorNote note1, EditorNote note2)
    {
        if(note1.bar < note2.bar)
        {
            return -1;
        }
        else if (note1.bar == note2.bar)
        {
            return note1.beat < note2.beat ? -1 : 1;
        }
        else
        {
            return 1;
        }
    }

    public void ModifyNote(Vector3 noteInfo)
    {
        targetNote = noteList[targetNoteIndex];
        targetNote.xPos = noteInfo.x;
        targetNote.transform.position = new Vector3(targetNote.xPos, targetNote.transform.position.y, targetNote.transform.position.z);
        targetNote.unitVecX = noteInfo.y;
        targetNote.unitVecY = noteInfo.z;
    }

    public void DeleteNote()
    {
        for (int i=0; i<noteList.Count; i++)
        {
            if (noteList[i] == targetNote)
            {
                noteList.RemoveAt(i);
                Destroy(targetNote.gameObject);
                targetNote = null;
                uiManager.InitiallizeNoteInfo();
                return;
            }
        }
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

    public void SheetSave()
    {
        List<NoteData> notes = new List<NoteData>();
        for(int i = 0; i < noteList.Count; i++)
        {
            NoteData note = new NoteData();
            note.xPos = noteList[i].xPos;
            note.unitVecX = noteList[i].unitVecX;
            note.unitVecY = noteList[i].unitVecY;
            note.beat = noteList[i].beat;
            note.bar = noteList[i].bar;
            notes.Add(note);
        }
        dataManager.FileSave(notes);
    }
}
