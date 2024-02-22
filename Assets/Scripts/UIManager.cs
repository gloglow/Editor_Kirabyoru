using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
using System.Windows.Forms;

public class UIManager : MonoBehaviour
{
    [SerializeField] private EditorManager editorManager;
    [SerializeField] private Player player;

    // UI set
    [SerializeField] private GameObject defaultUI;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject optionUI;

    // Button
    [SerializeField] private GameObject pauseBtn;
    [SerializeField] private GameObject resumeBtn;

    // note info UI
    [SerializeField] private TMP_InputField barInput;
    [SerializeField] private TMP_InputField beatInput;
    [SerializeField] private TMP_InputField xPosInput;
    [SerializeField] private TMP_InputField unitVecXInput;
    [SerializeField] private TMP_InputField unitVecYInput;

    // menu UI
    [SerializeField] private TextMeshProUGUI songText;

    // option UI
    [SerializeField] private TMP_InputField bpmInput;

    // for make, modify note.
    [SerializeField] private float minXpos;
    [SerializeField] private float maxXpos;
    private float tmpXPos;
    private int tmpBar;
    private float tmpBeat;
    private float tmpUnitVecX;
    private float tmpUnitVecY;

    private void Start()
    {
        InitiallizeNoteInfo();
        ChangeOption();
    }

    public void ChangeSongText(string text) // when music selected.
    {
        songText.text = text;
    }

    public void InitiallizeNoteInfo() // initialize note info UI.
    {
        barInput.text = string.Empty;
        beatInput.text = string.Empty;
        xPosInput.text = string.Empty;
        unitVecXInput.text = string.Empty;
        unitVecYInput.text = string.Empty;
    }

    public void ShowNoteInfo(EditorNote editorNote) // when note selected.
    {
        barInput.text = editorNote.bar.ToString();
        beatInput.text = editorNote.beat.ToString();
        xPosInput.text = editorNote.xPos.ToString();
        unitVecXInput.text = editorNote.unitVecX.ToString();
        unitVecYInput.text = editorNote.unitVecY.ToString();
    }

    private bool CheckValidNoteInput() // if note info input is valid, return true.
    {
        // if cant parse, system throw exception.
        // if not in valid range, throw exception.
        try
        {
            // minXpos < xPos < maxXpos
            tmpXPos = float.Parse(xPosInput.text);
            if(tmpXPos < minXpos || tmpXPos > maxXpos)
                throw new Exception();

            // bar : 1 ~
            tmpBar = int.Parse(barInput.text);
            if (tmpBar < 1)
                throw new Exception();

            // beat : 1, 1.25, 1.5, ~ 4.25, 4.5, 4.75
            tmpBeat = float.Parse(beatInput.text);
            if ((tmpBeat < 1 && tmpBeat < 5) || tmpBeat * 100 % 25 != 0)
                throw new Exception();

            tmpUnitVecX = float.Parse(unitVecXInput.text);
            tmpUnitVecY = float.Parse(unitVecYInput.text);
        }
        // if there is exception, open alert box.
        catch (Exception exception)
        {
            MessageBox.Show("Unvalid Input.\n" +
                "1. -9 < xpos < 9\n" +
                "2. 1 <= bar \n" +
                "3. 1 <= beat < 5 , beat = x.25 or x.50 or x.75");
            return false;
        }
        return true;
    }

    public void MakeNote() // when note make button pressed.
    {
        if(CheckValidNoteInput())
            editorManager.AddNote(tmpXPos, tmpBar, tmpBeat, tmpUnitVecX, tmpUnitVecY);
    }

    public void ModifyNoteInfo() // when note modify button pressed.
    {
        if(CheckValidNoteInput())
            editorManager.ModifyNote(tmpXPos, tmpBar, tmpBeat, tmpUnitVecX, tmpUnitVecY);
    }

    public void DeleteNote() // when note delete button pressed.
    {
        editorManager.DeleteNote();
    }

    public void ChangeOption() // option change.
    {
        player.bpm = int.Parse(bpmInput.text);
        player.secondPerBeat = 60 / player.bpm;
    }

    public void OnPauseBtn()
    {
        pauseBtn.gameObject.SetActive(true);
        resumeBtn.gameObject.SetActive(false);
    }

    public void OnResumeBtn()
    {
        pauseBtn.gameObject.SetActive(false);
        resumeBtn.gameObject.SetActive(true);
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
