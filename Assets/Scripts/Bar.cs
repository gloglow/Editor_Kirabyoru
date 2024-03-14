using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public int barNum;　//　何番目のバーなのか

    //　音楽の拍子
    [SerializeField] int theNumberOfBeats;　//　ビートの数。基本4/4のため、4で設定。（6/8→6、3/4→3等）
    [SerializeField] float theNumberOfSubbeats;　//　一つのビートを分ける下位ビートの数。4で設定すると1/16音符まで表現可能

    private void Start()
    {
        float subBeatGap = 1 / theNumberOfSubbeats;
        subBeatGap = Mathf.Floor(subBeatGap * 100f) / 100f;

        for (int i = 1; i <= theNumberOfBeats; i++) //　ビートにナンバリング
        {
            Transform obj = transform.GetChild(i - 1);
            TextMeshPro textMesh = obj.GetComponent<TextMeshPro>();
            textMesh.text = barNum.ToString() + "-" + i.ToString();

            for (int j = 0; j <= theNumberOfSubbeats - 1; j++)　//　下位ビートにバーとビートの情報を入れる
            {
                Transform childObj = obj.GetChild(j);
                SubBeat subBeat = childObj.GetComponent<SubBeat>();
                subBeat.barNum = barNum;
                subBeat.beatNum = i + (j * subBeatGap);
            }
        }
    }

}
