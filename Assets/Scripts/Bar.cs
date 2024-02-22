using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public int barNum; // n th bar

    private void Start()
    {
        for (int i = 1; i <= 4; i++)
        {
            // set each beat text ("n - n")
            Transform obj = transform.GetChild(i - 1);
            TextMeshPro textMesh = obj.GetComponent<TextMeshPro>();
            textMesh.text = barNum.ToString() + "-" + i.ToString();

            for (int j = 0; j <= 3; j++)
            {
                // set each subBeat barNum and beatNum
                Transform childObj = obj.GetChild(j);
                SubBeat subBeat = childObj.GetComponent<SubBeat>();
                subBeat.barNum = barNum;
                subBeat.beatNum = i + (j * 0.25f);
            }
        }
    }

}
