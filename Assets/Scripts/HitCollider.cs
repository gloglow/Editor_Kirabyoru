using UnityEngine;

public class HitCollider : MonoBehaviour
{
    public EditorManager editorManager;

    private void OnTriggerEnter(Collider other)
    {
        // when collide with note
        if (other.gameObject.layer == 8) // layer of note
        {
            Note note = other.gameObject.GetComponent<Note>();
            //ObjectPoolManager.Instance.notePool.Release(other.gameObject); // return note back to object pool.
        }
    }
}
