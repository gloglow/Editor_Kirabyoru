using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Ookii.Dialogs;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject defaultUI;
    public GameObject menuUI;
    public GameObject optionUI;
    public TextMeshProUGUI songText;

    VistaOpenFileDialog fileDialog;
    Stream openStream = null;

    private void Start()
    {
        fileDialog = new VistaOpenFileDialog();
    }

    public string MusicFileSelect()
    {
        fileDialog.Filter = "wav files (*.wav)|*.wav|mp3 files (*.mp3)|*.mp3";
        fileDialog.FilterIndex = 2;
        fileDialog.Title = "Music File Dialog";
        string fileAddress = FileOpen();
        Regex regex = new Regex(@"\..*$");
        songText.text = regex.Replace(Path.GetFileName(fileAddress), "");
        return fileAddress;
    }

    public string JsonFileSelect()
    {
        fileDialog.Filter = "json files (*.json)";
        fileDialog.FilterIndex = 1;
        fileDialog.Title = "json File Dialog";
        return FileOpen();
    }

    private string FileOpen()
    {
        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            if ((openStream = fileDialog.OpenFile()) != null)
            {
                return fileDialog.FileName;
            }
        }
        return null;
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
}
