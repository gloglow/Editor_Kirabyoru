using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public IObjectPool<GameObject> notePool { get; private set; }　//　ノーツのオブジェクトプール

    [SerializeField] private int noteMaxCnt;　//　基本生成されるノーツの数
    [SerializeField] private GameObject prefab_note;

    private void Awake()
    {
        Initialize(); //　オブジェクトプールとノーツを生成
    }

    public void Initialize()
    {
        // オブジェクトプールを生成
        notePool = new ObjectPool<GameObject>(CreateNote, BringNoteFromPool, ReturnNoteToPool
            , notdestroybuttmp, true, noteMaxCnt, noteMaxCnt);

        // ノーツを生成し、プールに入れる
        for (int i = 0; i< noteMaxCnt; i++)
        {
            Note note = CreateNote().GetComponent<Note>();
            note.notePool.Release(note.gameObject);
        }
    }

    public GameObject CreateNote()
    {
        GameObject note = Instantiate(prefab_note);
        note.GetComponent<Note>().notePool = notePool;
        return note;
    }

    public void BringNoteFromPool(GameObject note)
    {
        note.SetActive(true);
    }

    public void ReturnNoteToPool (GameObject note)
    {
        note.SetActive(false);
    }

    public void notdestroybuttmp(GameObject note)
    {

    }
}
