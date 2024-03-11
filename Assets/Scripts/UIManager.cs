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
    [SerializeField] private GameObject optionUI;

    // Button
    [SerializeField] private Image imageNoteModifyBtn;
    private bool modifyOnOff = false;
    [SerializeField] private GameObject pauseBtn;
    [SerializeField] private GameObject resumeBtn;

    // note info UI
    [SerializeField] private GameObject noteInfo;
    [SerializeField] private TextMeshProUGUI barBeatText;
    [SerializeField] private TextMeshProUGUI xPosText;
    [SerializeField] private TextMeshProUGUI unitVecXText;
    [SerializeField] private TextMeshProUGUI unitVecYText;

    // menu UI
    [SerializeField] private TextMeshProUGUI songText;

    // option UI
    [SerializeField] private TMP_InputField bpmInput;

    // help UI
    [SerializeField] private GameObject helpUI;

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
        barBeatText.text = string.Empty;
        xPosText.text = string.Empty;
        unitVecXText.text = string.Empty;
        unitVecYText.text = string.Empty;
    }

    public void ShowNoteInfo(EditorNote editorNote) // ノーツがクリックされた時
    {
        noteInfo.gameObject.SetActive(true);
        barBeatText.text = editorNote.bar.ToString() + " - " + editorNote.beat.ToString();
        xPosText.text = editorNote.xPos.ToString();
        unitVecXText.text = editorNote.unitVecX.ToString();
        unitVecYText.text = editorNote.unitVecY.ToString();
    }

    public void HideNoteInfo()
    {
        noteInfo.gameObject.SetActive(false);
    }

    public void OnHelpUI()
    {

    }

    public void OffHelpUI()
    {

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
        optionUI.SetActive(false);
    }

    public void OnMenu()
    {
        defaultUI.SetActive(false);
        optionUI.SetActive(false);
    }

    public void OnOptionUI()
    {
        optionUI.SetActive(true);
    }

    public void OffOptionUI()
    {
        optionUI.SetActive(false);
    }

    public void NoteMakeMode()
    {
        defaultUI.SetActive(false);
        optionUI.SetActive(false);
    }
}
