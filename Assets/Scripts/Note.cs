using UnityEngine;

public class Note : MonoBehaviour
{
    public Vector3 dirVec; // direction vector to move.

    // variables to calculate move speed.
    private Vector3 initialPos; // position when note is activated (spawner's position)
    public float initialTime; // time when note is activated
    private Vector3 destination; // on judge line.
    private float dist; // distance between initial position and destination.

    // range of perfect and great grade
    [SerializeField] private float perfectRange, greatRange; // 0.75, 0.83
    public float index;
    private float speed;

    public EditorManager editorManager;

    public int status; // 0 : idle, 1 : set destination and speed, 2 : move

    private void Update()
    {
        switch (status)
        {
            case 0:
                break;

            case 1: // when activated by stage manager
                speed = 1 /*gamemanager speed*/;
                initialPos = transform.position; // initial position (position of spawner)
                initialTime = (float)AudioSettings.dspTime; // activated timing
                
                RaycastHit rayHit;
                int layerMask = (1 << 7); // layer of real judge line
                
                if (Physics.Raycast(transform.position, dirVec, out rayHit, Mathf.Infinity, layerMask))
                {
                    destination = rayHit.point; // set destination on judge line
                    dist = Vector3.Distance(transform.position, destination); // distance to move.
                }

                // move.
                // calculate the position where this note should be at current timing.

                // speed = (current time / beat) * distance.
                // -> time taken for move initial position to destination is ONE BEAT. 
                float defaultSpeed = (((float)AudioSettings.dspTime - initialTime) / (editorManager.secondPerBeat * (1 / speed * 2)) * dist);

                // position = initial position + direction * speed * useroffset
                transform.position = initialPos + dirVec * defaultSpeed;
                status = 2; // change status into 2 (don't need to calculate distance)
                break;

            case 2:
                // moving mechanism is same.
                defaultSpeed= (((float)AudioSettings.dspTime - initialTime) / (editorManager.secondPerBeat * (1 / speed * 2))) * dist;
                transform.position = initialPos + dirVec * defaultSpeed;
                break;
        }
    }
    

    public void Exit()
    {
        // set note status 0 and return to note object pool.
        status = 0;
    }
}
