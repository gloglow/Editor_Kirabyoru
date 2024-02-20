using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public IObjectPool<GameObject> notePool { get; private set; } // object pool of notes.

    [SerializeField] private int noteMaxCnt; // the default number of notes. 
    [SerializeField] private GameObject prefab_note; // prefab of note.

    private void Awake()
    {
        Initialize(); // create object pool and objects.
    }

    public void Initialize()
    {
        // create note object pool.
        notePool = new ObjectPool<GameObject>(CreateNote, BringNoteFromPool, ReturnNoteToPool
            , notdestroybuttmp, true, noteMaxCnt, noteMaxCnt);

        // create notes and let them in pool.
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
