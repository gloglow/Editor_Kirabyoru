using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    public ObjectPoolManager objectPoolManager;

    private void OnTriggerEnter(Collider other)
    {
        // when collide with note
        if (other.gameObject.layer == 9) // layer of note
        {
            Note note = other.transform.GetComponent<Note>();
            note.Exit();
        }
    }
}
