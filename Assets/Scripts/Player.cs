using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField] private EditorManager editorManager;
    [SerializeField] private ObjectPoolManager poolManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;

    public float bpm; // 音楽の速さ
    [SerializeField] private int musicStartAfterBeats; // 再生ボタンを押し、音楽が始まるまでのディレイ。 8で設定
    public float secondPerBeat; // ビート当たりの時間。bpmで計算される
    [SerializeField] private int beatCnt; // ビートのカウンター
                                          
    private float startTime; // 音楽が始まった時間（オーディオシステム上の時間） 
    private float pauseTime; //　音楽が停止された時間
    private float lastBeatTime; // 以前のビートの時間

    private bool isPlay;　//　音楽がプレイされているかどうか
    private bool isPause;　//　音楽が中止されているかどうか

    [SerializeField] private float noteSpawnYPos; // ノーツが生成されるy位置。4で設定
    [SerializeField] private Vector3 playerCameraPos; // 音楽プレイモードのカメラの位置。(0, -2, -10)で設定
    
    private int index;　//　次生成されるノーツのインデックス
    private EditorNote crtNote;　//　生成されるノーツ


    private void Start()
    {
        Initialize();　//　初期化
    }

    private void Update()
    {
        if(isPlay)
        {
            // 以前ビートの時間から1ビートの時間が経ったらビートカウントを増加
            if (AudioSettings.dspTime - lastBeatTime > secondPerBeat)
            {
                beatCnt++;
                lastBeatTime += secondPerBeat;

                // 再生ボタンが押され、準備時間が過ぎたら音楽を再生
                if (beatCnt == musicStartAfterBeats + 1)
                {
                    audioManager.MusicPlay();
                }

                //　次のノーツが生成されるタイミングになったら生成
                while (editorManager.noteList.Count > index &&
                    (beatCnt - musicStartAfterBeats + 1) == (editorManager.noteList[index].bar - 1) * 4 + (int)editorManager.noteList[index].beat - 1)
                {
                    crtNote = editorManager.noteList[index];
                    float delayTime; // ノーツの生成を遅延させる時間。（1/4音符は０、1/8音符は0.5)
                    if (crtNote.beat == 1) // 1/4音符
                    {
                        delayTime = 0;
                    }
                    else // 1/8音符, 1/16音符
                    {
                        delayTime = (crtNote.beat - (int)crtNote.beat) * secondPerBeat * 0.5f;
                    }
                    StartCoroutine(MakeNote(delayTime + secondPerBeat * 0.5f, index));
                    index++;
                }
            }
        }
    }

    private void Initialize()　//　初期化
    {
        startTime = (float)AudioSettings.dspTime;
        lastBeatTime = startTime;

        isPlay = false;
        isPause = true;

        index = 0;
    }

    IEnumerator MakeNote(float time, int nIndex)　//　ノーツを生成
    {
        yield return new WaitForSeconds(time);　//　ディレイ時間間待機

        //　ノーツを生成
        GameObject obj = poolManager.notePool.Get();
        Note note = obj.GetComponent<Note>();
        note.transform.parent = transform;
        note.player = this;
        note.index = nIndex;

        EditorNote editorNote = editorManager.noteList[nIndex];

        //　ノーツを活性化
        note.status = 1;
        note.transform.position = new Vector3(editorNote.xPos, noteSpawnYPos, 0);
        note.dirVec = new Vector3(editorNote.unitVecX, editorNote.unitVecY, 0);
        
        if (isPause) note.status = 0;
    }

    public void MusicPlay()　//　音楽を再生
    {
        //　editor managerをプレイモードに転換
        editorManager.FlagPlayChange(true);
        editorManager.CameraPosMemory();
        editorManager.BarControl(false);

        Camera.main.transform.position = playerCameraPos;
        isPlay = true;
        isPause = false;
        lastBeatTime = (float)AudioSettings.dspTime;
        
    }

    public void MusicPause()　//　音楽を一時停止
    {
        if (!audioManager.IsMusicPlaying())
            return;

        isPlay = false;
        isPause = true;
        audioManager.MusicPause();

        // 一時停止された時間を記録
        pauseTime = (float)AudioSettings.dspTime;

        // 生成された全てのノーツの動作を中止
        for (int i = 0; i < transform.childCount; i++)
        {
            Note note = transform.GetChild(i).GetComponent<Note>();
            note.status = 0;
        }

        uiManager.OnResumeBtn();
    }

    public void Resume()　//　停止されてる音楽をまた再生
    {
        isPlay = true;
        isPause = false;
        
        // 停止されていた時間分、基準時間に反映
        lastBeatTime += (float)AudioSettings.dspTime - pauseTime;
        audioManager.MusicResume();

        // ノーツの動作を再活性化
        int childCnt = transform.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            Note note = transform.GetChild(i).GetComponent<Note>();
            note.initialTime += (float)AudioSettings.dspTime - pauseTime;
            note.status = 2;
        }

        uiManager.OnPauseBtn();
    }

    public void MusicStop()　//　音楽を中止
    {
        isPlay = false;
        isPause = true;

        index = 0;
        beatCnt = 0;

        editorManager.BackToEditorMode();

        audioManager.MusicStop();
        uiManager.OnPauseBtn();
    }
}
