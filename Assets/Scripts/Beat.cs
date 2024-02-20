using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Beat : MonoBehaviour
{
    [SerializeField] private GameObject prefab_PrimeBeat;
    [SerializeField] private GameObject prefab_SubBeat;
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float subBeatInterval;
    public List<SubBeat> beatParts = new List<SubBeat>();
    public int barNum;
    public int beatNum;

    private void Start()
    {
        /*
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = (barNum+1).ToString() + "-" + (beatNum+1).ToString();
        for (int i = 0; i < 4; i++)
        {
            GameObject obj;
            if(i == 0)
            {
                obj = Instantiate(prefab_PrimeBeat, transform);
            }
            else
            {
                obj = Instantiate(prefab_SubBeat, transform);
            }
            BeatPart beatPart = obj.GetComponent<BeatPart>();
            beatPart.transform.position = transform.position + new Vector3(0, i * subBeatInterval, 0);
            beatPart.barNum = barNum;
            beatPart.beatNum = beatNum + (i * 0.25f);
            beatParts.Add(beatPart);
        }*/
    }
}
