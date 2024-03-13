using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    public ObjectPoolManager objectPoolManager;

    private void OnTriggerEnter(Collider other)
    {
        // ノーツと衝突するとノーツを消す
        if (other.gameObject.layer == 9) // ノーツのレイヤー
        {
            Note note = other.transform.GetComponent<Note>();
            note.Exit();
        }
    }
}
