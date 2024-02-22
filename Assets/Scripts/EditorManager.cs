using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private Player player;
    [SerializeField] private TmpNote tmpNote;

    // prefab.
    [SerializeField] private GameObject prefab_bar;
    [SerializeField] private GameObject prefab_note;

    // bar
    [SerializeField] private GameObject bars;
    [SerializeField] private List<Bar> barList;

    // note.
    public List<EditorNote> noteList = new List<EditorNote>();

    private Vector2 mousePos; 
    [SerializeField] private Vector3 memoryCameraPos;
    [SerializeField] private int nextBarYPos; // when make new bar, its position. set (0, 4, 0) 
    
    public int targetNoteIndex;

    private bool flagMakeNote = false;
    private bool flagPlay = false;


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
                    if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
                    {
                        int rayHitLayer = rayHit.transform.gameObject.layer;
                        GameObject obj = rayHit.transform.gameObject;
                        if(rayHitLayer == 6)
                        {
                            bars.gameObject.SetActive(false);
                            uiManager.NoteMakeMode();
                            memoryCameraPos = Camera.main.transform.position;
                            Camera.main.transform.position = new Vector3(0, 0, -10);

                            SubBeat beatPart = obj.GetComponent<SubBeat>();
                            tmpNote.gameObject.SetActive(true);
                            tmpNote.bar = beatPart.barNum;
                            tmpNote.beat = beatPart.beatNum;
                            tmpNote.transform.position = new Vector3(0, 4, 0);
                            tmpNote.status = 0;
                            flagMakeNote = true;
                        }
                        else if (rayHitLayer == 8)
                        {
                            targetNoteIndex = FindNoteIndex(obj.GetComponent<EditorNote>());
                            if(targetNoteIndex != -1)
                                uiManager.ShowNoteInfo(noteList[targetNoteIndex]);
                        }
                    }                
                }
            }
        }
    }

    private int FindNoteIndex(EditorNote note) // find note index from noteList. if failed, return -1.
    {
        for(int i = 0; i<noteList.Count; i++)
        {
            if (noteList[i] == note)
                return i;
        }
        return -1;
    }

    public void BarControl(bool flag) // play mode -> bar off, editor mode -> bar on
    {
        bars.gameObject.SetActive(flag);
    }

    public void Initialize() // editor initialize. 
    {
        while(barList.Count > 0)
        {
            DeleteBar();
            uiManager.InitiallizeNoteInfo();
        }
    }

    public void BackToEditorMode()
    {
        flagPlay = false;
        Camera.main.transform.position = memoryCameraPos;
    }

    public void FlagPlayChange(bool flag)
    {
        flagPlay = flag;
    }

    private void ScreenDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Camera.main.transform.position += new Vector3(0, (mousePos.y - Input.mousePosition.y) * 0.0001f, 0);
        }
    }

    public void MakeBar()
    {
        GameObject obj = Instantiate(prefab_bar, bars.transform);
        Bar bar = obj.GetComponent<Bar>();
        bar.barNum = barList.Count + 1;
        bar.transform.position = new Vector3(0, nextBarYPos, 0);
        barList.Add(bar);
        nextBarYPos += 32;
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
        nextBarYPos -= 32;
    }

    public void AddNote(float xPos, int bar, float beat, float unitVecX, float unitVecY)
    {
        if (flagPlay)
        {
            flagPlay = false;
            player.MusicStop();
        }
        int actualBar = bar - 1;
        float actualBeat = beat - 1;
        while (barList.Count <= actualBar)
        {
            MakeBar();
        }
        GameObject obj = Instantiate(prefab_note);
        EditorNote editorNote = obj.GetComponent<EditorNote>();
        editorNote.xPos = xPos;
        editorNote.bar = bar;
        editorNote.beat = beat;
        editorNote.unitVecX = unitVecX;
        editorNote.unitVecY = unitVecY;
        editorNote.transform.parent = barList[actualBar].transform.GetChild((int)actualBeat).transform.GetChild((int)((actualBeat - (int)actualBeat) / 0.25f));
        editorNote.transform.position = new Vector3(editorNote.xPos, 0, 0);
        editorNote.transform.localPosition = new Vector3(editorNote.transform.localPosition.x, 0, 0);
        
        noteList.Add(editorNote);
        noteList.Sort(NoteCompare);
        flagMakeNote = false;
        bars.gameObject.SetActive(true);
        uiManager.BackToDefault();
        Camera.main.transform.position = memoryCameraPos;
    }

    private int NoteCompare(EditorNote note1, EditorNote note2) // sort notes by bar and beat.
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

    public void ModifyNote(float xPos, int bar, float beat, float unitVecX, float unitVecY)
    {
        if (flagPlay)
        {
            flagPlay = false;
            player.MusicStop();
        }
        noteList[targetNoteIndex].xPos = xPos;

        noteList[targetNoteIndex].bar = bar;
        noteList[targetNoteIndex].beat = beat;
        noteList[targetNoteIndex].unitVecX = unitVecX;
        noteList[targetNoteIndex].unitVecY = unitVecY;
        int actualBar = bar - 1;
        float actualBeat = beat - 1;
        while (barList.Count <= actualBar)
        {
            MakeBar();
        }

        noteList[targetNoteIndex].transform.parent = barList[actualBar].transform.GetChild((int)actualBeat).transform.GetChild((int)((actualBeat - (int)actualBeat) / 0.25f));
        noteList[targetNoteIndex].transform.position = new Vector3(noteList[targetNoteIndex].xPos, 0, 0);
        noteList[targetNoteIndex].transform.localPosition = new Vector3(noteList[targetNoteIndex].transform.localPosition.x, 0, 0);
    }

    public void DeleteNote()
    {
        if (flagPlay)
        {
            flagPlay = false;
            player.MusicStop();
        }
        if (targetNoteIndex == -1)
            return;

        Destroy(noteList[targetNoteIndex].gameObject);
        noteList.RemoveAt(targetNoteIndex);
        targetNoteIndex = -1;
        uiManager.InitiallizeNoteInfo();
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