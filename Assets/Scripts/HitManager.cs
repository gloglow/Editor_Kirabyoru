using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitManager : MonoBehaviour
{
    // Managing input. -> check whether input was in available area or not, and manage colliders and effects.

    [SerializeField] private GameObject prefab_hitCollider; // prefab of hit collider

    [SerializeField] private EditorManager editorManager;

    [SerializeField] private HitCollider[] hitColliders; // array to manage hit colliders

    [SerializeField] private TouchArea touchArea; // available touch area
    [SerializeField] private EditorJudgeLine judgeLine;

    [SerializeField] private int hitCnt; // the maximum number of available touch. I thought users will play with 2~4 fingers, so I set this variable 4.

    private void Start()
    {
        hitColliders = new HitCollider[hitCnt];

        // instantiate hit colliders and effects. and set their array.
        for (int i = 0; i < hitCnt; i++)
        {
            HitCollider hitCollider = Instantiate(prefab_hitCollider).GetComponent<HitCollider>();
            hitCollider.editorManager = editorManager;
            hitCollider.gameObject.SetActive(false);
            hitCollider.transform.SetParent(transform);
            hitColliders[i] = hitCollider;
        }
    }

    private void Update()
    {
        if (!editorManager.isPause)
        {
            // PC. 
            if (Input.GetMouseButtonDown(0))
            {
                // if there was mouse input, check whether input is in toucharea.
                Vector3 point = isinTouchArea(Input.mousePosition);

                if (point != new Vector3(999, 999, 999))
                {
                    // if input is in toucharea, calculate position where collider and effect will be made.
                    point = DecideColliderPos(point);
                }

                // PC only use mouse click so only need one collider and one effect.
                ActivateHit(0, point);
            }

            if (Input.GetMouseButtonUp(0))
            {
                // when mouseclick ended, hit collider off.
                hitColliders[0].gameObject.SetActive(false);
            }

            // Mobile.
            if (Input.touchCount > 0)
            {
                // there can be one touch or multiple touches.
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch tmpTouch = Input.GetTouch(i);
                    Vector3 point = isinTouchArea(tmpTouch);

                    if (point != new Vector3(999, 999, 999))
                    {
                        point = DecideColliderPos(point);
                    }

                    // when start touch, make collider and effect.
                    if (tmpTouch.phase == TouchPhase.Began)
                    {
                        ActivateHit(i, point);
                    }
                    // during touching, move collider to touch position.
                    else if (tmpTouch.phase == TouchPhase.Moved || tmpTouch.phase == TouchPhase.Stationary)
                    {
                        hitColliders[i].transform.position = point;
                    }
                    // when end touch, hit collider off.
                    else
                    {
                        hitColliders[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        
    }

    private Vector3 isinTouchArea(Vector3 mousepos)
    {
        // check whether input is in available touch area or not.

        Ray ray = Camera.main.ScreenPointToRay(mousepos);
        RaycastHit rayHit;
        int layerMask = (1 << 9); // toucharea layer
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, layerMask))
        {
            // if input is in toucharea, return the point.
            return rayHit.point;
        }
        else 
        { 
            // else, return meaningless vector.
            return new Vector3(999, 999, 999); 
        }
    }

    private Vector3 isinTouchArea(Touch touch) // overloading for mobile.
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit rayHit;
        int layerMask = (1 << 9);
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, layerMask))
        {
            return rayHit.point;
        }
        else
        {
            return new Vector3(999, 999, 999);
        }
    }

    private Vector3 DecideColliderPos(Vector3 point)
    {
        // touch area is wide than actual judgeline.
        // so calculate position of collider and effect to be on the judgeline for them.

        // calculate direction vector of ray.
        Vector3 dirVec = ((judgeLine.lineStartPos + judgeLine.lineEndPos) * 0.5f) - point;
        dirVec = Vector3.Normalize(dirVec);

        RaycastHit rayHit;
        int layerMask = (1 << 7); // layer of real judge line.

        if (Physics.Raycast(point, dirVec, out rayHit, Mathf.Infinity, layerMask) 
            || Physics.Raycast(point, -dirVec, out rayHit, Mathf.Infinity, layerMask))
        {
            // draw ray for two point with infinite length.
            // return rayHit.point that is on judgeline.
            return rayHit.point;
        }
        else
        {
            // else, return meaningless vector.
            return new Vector3(999, 999, 999);
        }
    }

    private void ActivateHit(int index, Vector3 pos)
    {
        // when there was touch, activate collider and effect.
        hitColliders[index].gameObject.SetActive(true);
        hitColliders[index].transform.position = pos;
    }
}
