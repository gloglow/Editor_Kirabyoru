using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class UIManager : MonoBehaviour
{
    public EditorManager editorManager;
    public GameObject defaultUI;
    public GameObject menuUI;
    public GameObject optionUI;
    public TextMeshProUGUI songText;
    [SerializeField] private TMP_InputField barInput;
    [SerializeField] private TMP_InputField beatInput;
    [SerializeField] private TMP_InputField xPosInput;
    [SerializeField] private TMP_InputField unitVecXInput;
    [SerializeField] private TMP_InputField unitVecYInput;
    [SerializeField] private TMP_InputField bpmInput;

    private void Start()
    {
        InitiallizeNoteInfo();
        ChangeOption();
    }
    public void ChangeSongText(string text)
    {
        songText.text = text;
    }
    
    public void InitiallizeNoteInfo()
    {
        barInput.text = string.Empty;
        beatInput.text = string.Empty;
        xPosInput.text = string.Empty;
        unitVecXInput.text = string.Empty;
        unitVecYInput.text = string.Empty;
    }

    public void ShowNoteInfo(Vector2 barBeat, float xPos, float unitVecX, float unitVecY)
    {
        barInput.text = barBeat.x.ToString();
        beatInput.text = barBeat.y.ToString();
        xPosInput.text = xPos.ToString();
        unitVecXInput.text = unitVecX.ToString();
        unitVecYInput.text = unitVecY.ToString();
    }

    public void MakeNote()
    {
        editorManager.AddNote(float.Parse(xPosInput.text), int.Parse(barInput.text), float.Parse(beatInput.text), float.Parse(unitVecXInput.text), float.Parse(unitVecYInput.text));
    }

    public void ModifyNoteInfo()
    {
        float xPos = float.Parse(xPosInput.text);
        if(Mathf.Abs(xPos) > 5)
        {
            xPos = xPos > 0 ? 5 : -5;
        }
        float unitVecX = float.Parse(unitVecXInput.text);
        float unitVecY = float.Parse(unitVecYInput.text);
        editorManager.ModifyNote(new Vector3(xPos, unitVecX, unitVecY));
    }

    public void ChangeOption()
    {
        editorManager.bpm = int.Parse(bpmInput.text);
        editorManager.secondPerBeat = 60 / editorManager.bpm;
    }

    public void BackToDefault()
    {
        defaultUI.SetActive(true);
        menuUI.SetActive(false);
        optionUI.SetActive(false);
    }

    public void OnMenu()
    {
        defaultUI.SetActive(false);
        menuUI.SetActive(true);
        optionUI.SetActive(false);
    }

    public void OnOption()
    {
        defaultUI.SetActive(false);
        menuUI.SetActive(false);
        optionUI.SetActive(true);
    }

    public void NoteMakeMode()
    {
        defaultUI.SetActive(false);
        menuUI.SetActive(false);
        optionUI.SetActive(false);
    }
}
