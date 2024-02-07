using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] private GameObject prefab_beat;
    public int num;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = Instantiate(prefab_beat, transform);
            Beat beat = obj.GetComponent<Beat>();
            beat.barNum = num;
            beat.beatNum = i + 1;
            beat.transform.localPosition += new Vector3(0, i * 2, 0);
        }
    }
}
