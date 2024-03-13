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

    // UIセット
    [SerializeField] private GameObject defaultUI;
    [SerializeField] private GameObject optionUI;

    // ボタン
    [SerializeField] private GameObject pauseBtn;
    [SerializeField] private GameObject resumeBtn;

    // ノーツ情報UI
    [SerializeField] private GameObject noteInfo;
    [SerializeField] private TextMeshProUGUI barBeatText;
    [SerializeField] private TextMeshProUGUI xPosText;
    [SerializeField] private TextMeshProUGUI unitVecXText;
    [SerializeField] private TextMeshProUGUI unitVecYText;

    // メニューUI
    [SerializeField] private TextMeshProUGUI songText;

    // オプションUI
    [SerializeField] private TMP_InputField bpmInput;

    private void Start()
    {
        InitiallizeNoteInfo();　// ノーツ情報を初期化
        ChangeOption();　//　オプションの初期状態を適用
    }

    public void ChangeSongText(string text) // 音楽のタイトルを表示
    {
        songText.text = text;
    }

    public void InitiallizeNoteInfo() // ノーツ情報を初期化
    {
        barBeatText.text = string.Empty;
        xPosText.text = string.Empty;
        unitVecXText.text = string.Empty;
        unitVecYText.text = string.Empty;
    }

    public void ShowNoteInfo(EditorNote editorNote) // ノーツがクリックされた時、ノーツ情報を表示
    {
        noteInfo.gameObject.SetActive(true);
        barBeatText.text = editorNote.bar.ToString() + " - " + editorNote.beat.ToString();
        xPosText.text = editorNote.xPos.ToString();
        unitVecXText.text = editorNote.unitVecX.ToString();
        unitVecYText.text = editorNote.unitVecY.ToString();
    }

    public void HideNoteInfo()　//　ノーツ情報を非表示
    {
        noteInfo.gameObject.SetActive(false);
    }

    public void ChangeOption() //　変わったオプションを適用
    {
        player.bpm = int.Parse(bpmInput.text);
        player.secondPerBeat = 60 / player.bpm;
    }

    public void OnPauseBtn()　//　pauseボタン活性化、resumeボタン非活性化
    {
        pauseBtn.gameObject.SetActive(true);
        resumeBtn.gameObject.SetActive(false);
    }

    public void OnResumeBtn()　//　pauseボタン非活性化、resumeボタン活性化
    {
        pauseBtn.gameObject.SetActive(false);
        resumeBtn.gameObject.SetActive(true);
    }

    public void BackToDefault()　//　ディフォルトUI
    {
        defaultUI.SetActive(true);
        optionUI.SetActive(false);
    }

    public void OnOptionUI()　//　オプションUIを表示
    {
        optionUI.SetActive(true);
    }

    public void OffOptionUI()　//　オプションUIを非表示
    {
        optionUI.SetActive(false);
    }

    public void NoteMakeMode()　//　ノーツ生成モード
    {
        defaultUI.SetActive(false);
        optionUI.SetActive(false);
    }
}
