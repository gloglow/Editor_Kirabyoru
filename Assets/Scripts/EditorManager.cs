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

    // プレハブ
    [SerializeField] private GameObject prefab_bar;
    [SerializeField] private GameObject prefab_note;

    // バー
    [SerializeField] private GameObject bars;
    [SerializeField] private List<Bar> barList;

    // ノーツ関連
    public List<EditorNote> noteList = new List<EditorNote>();
    public int targetNoteIndex;　//　現在選択しているノーツ

    // カメラコントロール
    private Vector2 mousePos; 
    [SerializeField] private Vector3 memoryCameraPos;
    [SerializeField] private int barInterval; // バー間間隔。8で設定
    [SerializeField] private int nextBarYPos; // 次のバーが生成される位置のy座標
    [SerializeField] float scrollSpeed; // ドラッグして画面を動かす時の速度。0.0002で設定

    // エディター動作制御
    private bool isPlayMode = false;　//　音楽プレイモードかどうか
    public int status;　//　エディター動作状態
    private GameObject rayHitObj;　//　ユーザーがクリックしたオブジェクト


    void Update()
    {
        switch (status)
        {
            case 0: // 基本状態
                
                //　ドラッグ：画面スクロール
                if (Input.GetMouseButtonDown(0))
                {
                    mousePos = Input.mousePosition;
                }
                if (!isPlayMode && Input.GetMouseButton(0))
                {
                    Camera.main.transform.position += new Vector3(0, (mousePos.y - Input.mousePosition.y) * scrollSpeed, 0);
                }
                
                //　ユーザーがクリックしたオブジェクトの種類をチェック
                if ((Input.GetMouseButtonUp(0))
                && (EventSystem.current.IsPointerOverGameObject() == false))
                {
                    if (Vector3.Distance(mousePos, Input.mousePosition) < 1)　//　クリックした状態で１以上マウスを動かしたら「クリック」じゃなく「ドラッグ」で認識
                    {
                        int rayHitLayer = LayerFromMouseRay();　//　クリックしたオブジェクトのレイヤー
                        switch (rayHitLayer)
                        {
                            case 6: // バーをクリックした場合：ノーツを生成
                                UnSelectNote();
                                MakeNote(rayHitObj);　//　クリックしたバーの上にノーツを生成
                                status = 1;
                                break;
                            case 8: //　エディターモードでノーツをクリックした場合：ノーツを選択し、ノーツの編集ができるようにする
                                SelectNote(FindNoteIndex(rayHitObj.GetComponent<EditorNote>()));
                                status = 2;
                                break;
                            case 9: //　音楽プレイモードでノーツをクリックした場合：ノーツを選択
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
            case 1: // バーがクリックされた場合：ノーツを生成するために他の作動は中止

                break;
            case 2: // エディターモードでノーツをクリックした場合
                if (Input.GetMouseButtonDown(0) && (EventSystem.current.IsPointerOverGameObject() == false))
                {
                    if (LayerFromMouseRay() != 8)　//　ノーツではないものをクリック：選択解除
                    {
                        UnSelectNote();
                        mousePos = Input.mousePosition;
                        status = 0;
                        break;
                    }
                }
                if (Input.GetMouseButton(0) && (EventSystem.current.IsPointerOverGameObject() == false))　//　ノーツをドラッグ：ノーツ移動
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

    public void CameraPosMemory()　//　現在のカメラの位置を記憶
    {
        memoryCameraPos = Camera.main.transform.position;
    }

    public void BackToEditorMode() //　音楽プレイモード　→　エディターモード
    {
        isPlayMode = false;
        BarControl(true);
        for (int i = 0; i < transform.childCount; i++)　//　プレイモードのノーツを全部日活性化
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                Note note = transform.GetChild(i).GetComponent<Note>();
                note.Exit();
            }
        }
        Camera.main.transform.position = memoryCameraPos;　//　記憶しておいた位置にカメラを移動
    }

    public void EditNoteDirection()　//　ノーツの方向ベクトル編集
    {
        if (targetNoteIndex == -1)　//　選択しているノーツがなければ
            return;
        if (isPlayMode)　//　音楽プレイモード→エディターモード
            player.MusicStop();

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
    }

    private int LayerFromMouseRay()　//　マウスからのrayに打たれたオブジェクトのレイヤー
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        int rayHitLayer = -1;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
        {
            rayHitLayer = rayHit.transform.gameObject.layer;
            rayHitObj = rayHit.transform.gameObject;
        }
        return rayHitLayer;
    }

    private void MakeNote(GameObject rayHit)　//　ノーツを生成
    {
        bars.gameObject.SetActive(false);
        uiManager.NoteMakeMode();
        CameraPosMemory();
        Camera.main.transform.position = new Vector3(0, 0, -10);

        SubBeat beatPart = rayHit.GetComponent<SubBeat>();
        tmpNote.gameObject.SetActive(true);
        tmpNote.bar = beatPart.barNum;
        tmpNote.beat = beatPart.beatNum;
        tmpNote.transform.position = new Vector3(0, 4, 0);
        tmpNote.status = 0;
        tmpNote.flagMake = true;
    }

    private void SelectNote(int index)　//　ノーツを選択
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

    private void UnSelectNote()　//　ノーツの選択を解除
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

    private int FindNoteIndex(EditorNote note)　//　ノーツのインデックスを探す
    {
        for(int i = 0; i<noteList.Count; i++)
        {
            if (noteList[i] == note)
                return i;
        }
        return -1;
    }

    private void EditNotePos()　//　ノーツの位置を編集
    {
        if (targetNoteIndex == -1)
            return;

        EditorNote editorNote = noteList[targetNoteIndex];

        float xPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        xPos = Mathf.RoundToInt(xPos);

        int bar = editorNote.bar;
        float beat = editorNote.beat;

        if(LayerFromMouseRay() == 6) //　バーのレイヤー
        {
            SubBeat subBeat = rayHitObj.GetComponent<SubBeat>();
            bar = subBeat.barNum;
            beat = subBeat.beatNum;
        }

        noteList[targetNoteIndex] = NoteSetting(editorNote, xPos, bar, beat, editorNote.unitVecX, editorNote.unitVecY);
        uiManager.ShowNoteInfo(noteList[targetNoteIndex]);
    }


    public void BarControl(bool flag)　//　バーを活性化・非活性化
    {
        bars.gameObject.SetActive(flag);
    }

    public void Initialize()　//　エディターを初期化 
    {
        while(barList.Count > 0)
        {
            DeleteBar();
            uiManager.InitiallizeNoteInfo();
        }
    }

    public void FlagPlayChange(bool flag)
    {
        isPlayMode = flag;
    }

    public void MakeBar() // バーを生成
    {
        GameObject obj = Instantiate(prefab_bar, bars.transform);
        Bar bar = obj.GetComponent<Bar>();
        bar.barNum = barList.Count + 1;
        bar.transform.position = new Vector3(0, nextBarYPos, 0);
        barList.Add(bar);
        nextBarYPos += barInterval;
    }

    public void DeleteBar() // バーを削除する
    {
        if (barList.Count <= 0)
            return;

        // 削除するバーの中にあるノーツを削除
        for (int i = noteList.Count - 1; i >= 0; i--)
        {
            if (noteList[i].bar >= barList.Count)
            {
                DeleteNote(i);
            }
            else
                break;
        }

        // バーを削除
        Destroy(barList[barList.Count - 1].gameObject);
        barList.RemoveAt(barList.Count - 1);
        nextBarYPos -= barInterval;
    }

    public void AddNote(float xPos, int bar, float beat, float unitVecX, float unitVecY)　//　ノーツを追加
    {
        BarMakeForNoteMaking(bar - 1);　//　必要により、ノーツを追加するためにバーを生成する

        // ノーツ生成
        GameObject obj = Instantiate(prefab_note);
        EditorNote editorNote = obj.GetComponent<EditorNote>();
        editorNote = NoteSetting(editorNote, xPos, bar, beat, unitVecX, unitVecY);
        editorNote.arrow.transform.rotation = Quaternion.Euler(0, 0, unitVecX * 90f);
        noteList.Add(editorNote);
        noteList.Sort(NoteCompare);

        //　追加したノーツを選択
        targetNoteIndex = FindNoteIndex(editorNote);
        if(targetNoteIndex != -1)
        {
            UnSelectNote();
        }
        SelectNote(targetNoteIndex);

        BarControl(true);
        uiManager.BackToDefault();
        Camera.main.transform.position = memoryCameraPos;
    }

    public void ModifyNoteDir(float xVec, float yVec)　//　ノーツの方向ベクトルを修正
    {
        noteList[targetNoteIndex].unitVecX = xVec;
        noteList[targetNoteIndex].unitVecY = yVec;
        noteList[targetNoteIndex].arrow.transform.rotation = Quaternion.Euler(0, 0, xVec * 90f);

        Camera.main.transform.position = new Vector3(0, noteList[targetNoteIndex].transform.position.y, -10);
        BarControl(true);
        uiManager.BackToDefault();
        uiManager.ShowNoteInfo(noteList[targetNoteIndex]);
    }

    private int NoteCompare(EditorNote note1, EditorNote note2) // ノーツリストをbarとbeatで整列するための関数
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

    private void BarMakeForNoteMaking(int actualBar)
    {
        while (barList.Count <= actualBar)
        {
            MakeBar();
        }
    }

    private EditorNote NoteSetting(EditorNote editorNote, float xPos, int bar, float beat, float unitVecX, float unitVecY)　//　伝達もらった値でノーツをセッティング
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

    public void ReadyToDeleteNote() //　ノーツを削除
    {
        if (isPlayMode)
        {
            player.MusicStop();
        }
        if (targetNoteIndex == -1)
            return;

        uiManager.HideNoteInfo();
        DeleteNote(targetNoteIndex);
    }

    private void DeleteNote(int index)　//　インデックスを確認し、ノーツを削除
    {
        Camera.main.transform.position = new Vector3(0, noteList[index].transform.position.y, -10); //　削除されたノーツの位置にカメラを移動
        Destroy(noteList[index].gameObject);
        noteList.RemoveAt(index);

        if(targetNoteIndex == index)
        {
            targetNoteIndex = -1;
            uiManager.InitiallizeNoteInfo();
        }
    }

    public void SheetSave() // ノーツリストを作り、譜面ファイルを生成するためにdata managerに渡す 
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

    public void Quit()　//　ゲーム終了
    {
        UnityEngine.Application.Quit();
    }
}