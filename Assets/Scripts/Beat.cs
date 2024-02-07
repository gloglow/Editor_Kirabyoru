using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Beat : MonoBehaviour
{
    [SerializeField] private TextMeshPro numText;
    public int barNum;
    public int beatNum;

    private void Start()
    {
        numText = GetComponent<TextMeshPro>();
        numText.text = barNum.ToString() +  "-" + beatNum.ToString();
    }


}
