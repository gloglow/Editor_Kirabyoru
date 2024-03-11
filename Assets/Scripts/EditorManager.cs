using Ookii.Dialogs;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
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
    [SerializeField] private int barInterval; // interval between bars. set 8
    [SerializeField] private int nextBarYPos; // when make new bar, its position. set (0, 4, 0) 
    
    public int targetNoteIndex;

    private bool makeNoteMode = false;
    private bool playMode = false;

    [SerializeField] float scrollSpeed; // set 0.0002

    public int status;
    private GameObject rayHitObj;


    void Update()
    {
        switch (status)
        {
            case 0: // 基本状態
                
                if (Input.GetMouseButtonDown(0))
                {
                    mousePos = Input.mousePosition;
                }
                if (!playMode && Input.GetMouseButton(0))
                {
                    Camera.main.transform.position += new Vector3(0, (mousePos.y - Input.mousePosition.y) * scrollSpeed, 0);
                }
                
                if ((Input.GetMouseButtonUp(0))
                && (EventSystem.current.IsPointerOverGameObject() == false))
                {
                    if (Vector3.Distance(mousePos, Input.mousePosition) < 1)
                    {
                        int rayHitLayer = LayerFromMouseRay();
                        switch (rayHitLayer)
                        {
                            case 6: // バー
                                UnSelectNote();
                                MakeNote(rayHitObj);
                                status = 1;
                                break;
                            case 8: //　エディターノーツ
                                SelectNote(FindNoteIndex(rayHitObj.GetComponent<EditorNote>()));
                                status = 2;
                                break;
                            case 9: //　ノーツ
                                Note note = rayHitObj.GetComponent<Note>();
                                SelectNote(note.index);
                                break;
                            default:
                                UnSelectNote();
                                break;
                        }
                    }
                }
                break;
            case 1: // バーがクリックされた時（ノーツを作る）

                break;
            case 2: // ノーツがクリックされた時
                if (Input.GetMouseButtonDown(0) && (EventSystem.current.IsPointerOverGameObject() == false))
                {
                    if (LayerFromMouseRay() != 8)
                    {
                        UnSelectNote();
                        mousePos = Input.mousePosition;
                        status = 0;
                        break;
                    }
                }
                if (Input.GetMouseButton(0) && (EventSystem.current.IsPointerOverGameObject() == false))
                {
                    EditNotePos();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    status = 0;
                }
                break;
        }
    }

    public void CameraPosMemory()
    {
        memoryCameraPos = Camera.main.transform.position;
    }

    public void BackToEditorMode()
    {
        playMode = false;
        BarControl(true);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                Note note = transform.GetChild(i).GetComponent<Note>();
                note.Exit();
            }
        }
        Camera.main.transform.position = memoryCameraPos;
    }

    public void EditNoteDirection()
    {
        if (targetNoteIndex == -1)
            return;
        if (playMode)
        {
            player.MusicStop();
        }
        status = 1;
        BarControl(false);
        uiManager.NoteMakeMode();
        memoryCameraPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        tmpNote.gameObject.SetActive(true);
        tmpNote.transform.position = new Vector3(0, 4, 0);
        tmpNote.transform.position = new Vector3(noteList[targetNoteIndex].xPos, tmpNote.transform.position.y, tmpNote.transform.position.z);
        tmpNote.status = 0;
        tmpNote.flagMake = false;
        makeNoteMode = true;
    }

    private int LayerFromMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        int rayHitLayer = -1;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
        {
            rayHitLayer = rayHit.transform.gameObject.layer;
            rayHitObj = rayHit.transform.gameObject;
        }
        Debug.Log(rayHitLayer);
        return rayHitLayer;
    }

    private void MakeNote(GameObject rayHit)
    {
        bars.gameObject.SetActive(false);
        uiManager.NoteMakeMode();
        memoryCameraPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        SubBeat beatPart = rayHit.GetComponent<SubBeat>();
        tmpNote.gameObject.SetActive(true);
        tmpNote.bar = beatPart.barNum;
        tmpNote.beat = beatPart.beatNum;
        tmpNote.transform.position = new Vector3(0, 4, 0);
        tmpNote.status = 0;
        tmpNote.flagMake = true;
        makeNoteMode = true;
    }

    private void SelectNote(int index)
    {
        if(targetNoteIndex != -1)
        {
            UnSelectNote();
        }
        targetNoteIndex = index;
        if (targetNoteIndex != -1)
        {
            MeshRenderer meshRenderer = noteList[targetNoteIndex].GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.red;
            uiManager.ShowNoteInfo(noteList[targetNoteIndex]);
        }
    }

    private void UnSelectNote()
    {
        if(targetNoteIndex == -1)
        {
            return;
        }
        MeshRenderer meshRenderer = noteList[targetNoteIndex].GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.blue;
        targetNoteIndex = -1;
        uiManager.HideNoteInfo();
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

    private void EditNotePos()
    {
        if (targetNoteIndex == -1)
            return;
        EditorNote editorNote = noteList[targetNoteIndex];
        float xPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        xPos = Mathf.RoundToInt(xPos);
        int bar = editorNote.bar;
        float beat = editorNote.beat;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
        {
            int rayHitLayer = rayHit.transform.gameObject.layer;
            GameObject obj = rayHit.transform.gameObject;
            if (rayHitLayer == 6) // バーがクリックされた時
            {
                SubBeat subBeat = obj.GetComponent<SubBeat>();
                bar = subBeat.barNum;
                beat = subBeat.beatNum;
            }
        }
        noteList[targetNoteIndex] = NoteSetting(editorNote, xPos, bar, beat, editorNote.unitVecX, editorNote.unitVecY);
        uiManager.ShowNoteInfo(noteList[targetNoteIndex]);
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

    public void FlagPlayChange(bool flag)
    {
        playMode = flag;
    }

    private void ScreenDrag() // in editor mode, scroll by mouse
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Camera.main.transform.position += new Vector3(0, (mousePos.y - Input.mousePosition.y) * scrollSpeed, 0);
        }
    }

    public void MakeBar() // when make bar button pressed.
    {
        GameObject obj = Instantiate(prefab_bar, bars.transform);
        Bar bar = obj.GetComponent<Bar>();
        bar.barNum = barList.Count + 1;
        bar.transform.position = new Vector3(0, nextBarYPos, 0);
        barList.Add(bar);
        nextBarYPos += barInterval;
    }

    public void DeleteBar() // when delete bar button pressed.
    {
        if (barList.Count <= 0)
            return;

        // remove notes on bar.
        for (int i = noteList.Count - 1; i >= 0; i--)
        {
            if (noteList[i].bar >= barList.Count)
            {
                DeleteNote(i);
            }
            else
                break;
        }

        // remove bar.
        Destroy(barList[barList.Count - 1].gameObject);
        barList.RemoveAt(barList.Count - 1);
        nextBarYPos -= barInterval;
    }

    public void AddNote(float xPos, int bar, float beat, float unitVecX, float unitVecY)
    {
        // if there isnt bar
        BarMakeForNoteMaking(bar - 1);

        // make note
        GameObject obj = Instantiate(prefab_note);
        EditorNote editorNote = obj.GetComponent<EditorNote>();
        editorNote = NoteSetting(editorNote, xPos, bar, beat, unitVecX, unitVecY);
        editorNote.arrow.transform.rotation = Quaternion.Euler(0, 0, unitVecX * 90f);
        noteList.Add(editorNote);
        noteList.Sort(NoteCompare);
        targetNoteIndex = FindNoteIndex(editorNote);
        
        if(targetNoteIndex != -1)
        {
            UnSelectNote();
        }
        makeNoteMode = false;
        bars.gameObject.SetActive(true);
        uiManager.BackToDefault();
        Camera.main.transform.position = memoryCameraPos;
        SelectNote(targetNoteIndex);
        uiManager.ShowNoteInfo(editorNote);
    }

    public void ModifyNoteDir(float xVec, float yVec)
    {
        noteList[targetNoteIndex].unitVecX = xVec;
        noteList[targetNoteIndex].unitVecY = yVec;
        noteList[targetNoteIndex].arrow.transform.rotation = Quaternion.Euler(0, 0, xVec * 90f);
        makeNoteMode = false;
        bars.gameObject.SetActive(true);
        Camera.main.transform.position = new Vector3(0, noteList[targetNoteIndex].transform.position.y, -10);
        
        uiManager.BackToDefault();
        uiManager.ShowNoteInfo(noteList[targetNoteIndex]);
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

    public void ModifyNote(float xPos, int bar, float beat, float unitVecX, float unitVecY) // modify button pressed.
    {
        // if there isnt bar
        BarMakeForNoteMaking(bar - 1);

        // note modify.
        noteList[targetNoteIndex] = NoteSetting(noteList[targetNoteIndex], xPos, bar, beat, unitVecX, unitVecY);
    }

    private void BarMakeForNoteMaking(int actualBar)
    {
        while (barList.Count <= actualBar)
        {
            MakeBar();
        }
    }

    private EditorNote NoteSetting(EditorNote editorNote, float xPos, int bar, float beat, float unitVecX, float unitVecY)
    {
        editorNote.xPos = xPos;
        editorNote.bar = bar;
        editorNote.beat = beat;
        editorNote.unitVecX = unitVecX;
        editorNote.unitVecY = unitVecY;

        int actualBar = bar - 1;
        float actualBeat = beat - 1;
        editorNote.transform.parent = barList[actualBar].transform.GetChild((int)actualBeat).transform.GetChild((int)((actualBeat - (int)actualBeat) / 0.25f));
        editorNote.transform.position = new Vector3(editorNote.xPos, 0, 0);
        editorNote.transform.localPosition = new Vector3(editorNote.transform.localPosition.x, 0, 0);

        return editorNote;
    }

    public void DeleteNote() // delete button pressed.
    {
        if (playMode)
        {
            player.MusicStop();
        }
        if (targetNoteIndex == -1)
            return;
        uiManager.HideNoteInfo();
        DeleteNote(targetNoteIndex);
    }

    private void DeleteNote(int index)
    {
        Camera.main.transform.position = new Vector3(0, noteList[index].transform.position.y, -10);
        Destroy(noteList[index].gameObject);
        noteList.RemoveAt(index);
        if(targetNoteIndex == index)
        {
            targetNoteIndex = -1;
            uiManager.InitiallizeNoteInfo();
        }
    }

    public void SheetSave() // save current note list into json file. 
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

    public void Quit()
    {
        UnityEngine.Application.Quit();
    }
}