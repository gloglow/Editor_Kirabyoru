using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    // managers
    [SerializeField] private EditorManager editorManager;
    [SerializeField] private ObjectPoolManager poolManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;

    public float bpm; // music bpm.
    [SerializeField] private int musicStartAfterBeats; // the number of initial beats. set 8.
    public float secondPerBeat; // second per beat. calculated by bpm.
    [SerializeField] private int beatCnt; // counting beat.
                                          // 
    private float startTime; // start timing of audio system. 
    private float lastBeatTime; // timing of last beat.

    private bool flagPlay;
    private bool isPause;
    private float pauseTimer; // save the time when pause button is pressed.

    [SerializeField] private float spawnYPos; // note spawn position. set 4.
    private int crtIndex;
    private int index;
    [SerializeField] private Vector3 defaultCameraPos; // default camera position. set (0, -2, -10)

    private EditorNote editorNote;

    public int off;
    private void Start()
    {
        flagPlay = false;
        startTime = (float)AudioSettings.dspTime;
        lastBeatTime = startTime;
        isPause = true;
        index = 0;
    }

    private void Update()
    {
        if(flagPlay)
        {
            if ((Input.GetMouseButtonUp(0))
                && (EventSystem.current.IsPointerOverGameObject() == false))
            {
                Vector3 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
                {
                    int rayHitLayer = rayHit.transform.gameObject.layer;
                    GameObject obj = rayHit.transform.gameObject;
                    if (rayHitLayer == 9)
                    {
                        Note note = obj.GetComponent<Note>();
                        editorManager.targetNoteIndex = note.index;
                        uiManager.ShowNoteInfo(editorManager.noteList[note.index]);
                    }

                }
            }

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
                while (editorManager.noteList.Count > index &&
                    (beatCnt - musicStartAfterBeats + 1) == (editorManager.noteList[index].bar - 1) * 4 + (int)editorManager.noteList[index].beat - 1)
                {
                    editorNote = editorManager.noteList[index];
                    float f; // the waiting time of note. (if note is 1/4 note, not wait.)
                    if (editorNote.beat == 1) // 1/4 note.
                    {
                        f = 0;
                    }
                    else // 1/8 note, 1/16 note, etc
                    {
                        // if beat of a note is 6.5, at 6th beat, wait 0.5beat time and be made.
                        f = (editorNote.beat - (int)editorNote.beat) * secondPerBeat * 0.5f;
                    }
                    
                    StartCoroutine(MakeNote(f, index));
                    index++;
                }
            }
        }
    }

    IEnumerator MakeNote(float time, int nIndex)
    {
        yield return new WaitForSeconds(time);
        // bring note from note object pool.
        GameObject obj = poolManager.notePool.Get();
        Note note = obj.GetComponent<Note>();
        note.transform.parent = transform;
        note.player = this;
        note.index = nIndex;
        //note.initialTime = (float)AudioSettings.dspTime;

        EditorNote editorNote = editorManager.noteList[nIndex];
        // activate note.
        note.status = 1;
        note.transform.position = new Vector3(editorNote.xPos, spawnYPos, 0);
        note.dirVec = new Vector3(editorNote.unitVecX, editorNote.unitVecY, 0);
        crtIndex++;
        if (isPause) note.status = 0;
    }

    public void MusicPlay()
    {
        editorManager.FlagPlayChange(true);
        editorManager.CameraPosMemory();
        Camera.main.transform.position = defaultCameraPos;
        flagPlay = true;
        lastBeatTime = (float)AudioSettings.dspTime;
        isPause = false;
        editorManager.BarControl(false);
    }

    public void MusicPause()
    {
        if (!audioManager.IsMusicPlaying())
        {
            return;
        }

        isPause = true;
        flagPlay = false;
        audioManager.MusicPause();

        // record the time when pause button is pressed.
        pauseTimer = (float)AudioSettings.dspTime;

        // all of notes stop when pause button is pressed.
        for (int i = 0; i < transform.childCount; i++)
        {
            Note note = transform.GetChild(i).GetComponent<Note>();
            note.status = 0;
        }

        uiManager.OnResumeBtn();
    }

    public void Resume()
    {
        // unpause.
        isPause = false;
        flagPlay = true;

        // add the time passed during pause to lastBeatTime.
        lastBeatTime += (float)AudioSettings.dspTime - pauseTimer;
        audioManager.MusicResume();

        // reactivate notes.
        int childCnt = transform.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            Note note = transform.GetChild(i).GetComponent<Note>();
            note.initialTime += (float)AudioSettings.dspTime - pauseTimer;
            note.status = 2;
        }

        uiManager.OnPauseBtn();
    }

    public void MusicStop()
    {
        flagPlay = false;
        isPause = true;

        crtIndex = 0;
        index = 0;
        beatCnt = 0;

        editorManager.BackToEditorMode();

        audioManager.MusicStop();
        uiManager.OnPauseBtn();
    }
}
