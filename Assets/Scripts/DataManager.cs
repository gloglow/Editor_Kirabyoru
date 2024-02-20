using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using LitJson;
using Ookii.Dialogs;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using System.Runtime.InteropServices.WindowsRuntime;

public class DataManager : MonoBehaviour
{
    [SerializeField] private EditorManager editorManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject prefab_EditorNote;
    private string noteDataFilePath = UnityEngine.Application.streamingAssetsPath;

    VistaOpenFileDialog openDialog;
    VistaSaveFileDialog saveDialog;
    Stream openStream = null;

    private void Start()
    {
        openDialog = new VistaOpenFileDialog();
        saveDialog = new VistaSaveFileDialog();
    }

    public string MusicFileSelect()
    {
        openDialog.Filter = "wav files (*.wav)|*.wav|mp3 files (*.mp3)|*.mp3";
        openDialog.FilterIndex = 2;
        openDialog.Title = "Music File Dialog";
        string fileAddress = FileOpen();
        Regex regex = new Regex(@"\..*$");
        uiManager.ChangeSongText(regex.Replace(Path.GetFileName(fileAddress), ""));
        return fileAddress;
    }

    public string JsonFileSelect()
    {
        openDialog.Filter = "json files (*.json)|*.json";
        openDialog.FilterIndex = 1;
        openDialog.Title = "json File Dialog";
        return FileOpen();
    }

    private string FileOpen()
    {
        if (openDialog.ShowDialog() == DialogResult.OK)
        {
            if ((openStream = openDialog.OpenFile()) != null)
            {
                return openDialog.FileName;
            }
        }
        return null;
    }

    public void FileSave(List<NoteData> noteList)
    {
        JsonData jsondata = JsonMapper.ToJson(noteList);

        saveDialog.Filter = "json files (*.json)|*.json";
        openDialog.FilterIndex = 1;
        openDialog.Title = "json File Dialog";
        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(saveDialog.FileName + ".json", jsondata.ToString());
        }
    }
    
    public void SheetSelect()
    {
        LoadSheetFile(JsonFileSelect());
    }

    public void LoadSheetFile(string fileAddress) // Load JSON file having Note information.
    {
        if (File.Exists(fileAddress))
        {
            var fileText = File.ReadAllText(fileAddress);
            JsonData jData = JsonMapper.ToObject(fileText.ToString());
            editorManager.Initialize();
            uiManager.InitiallizeNoteInfo();
            for (int i = 0; i < jData.Count; i++)
            {
                float xPos = float.Parse(jData[i]["xPos"].ToString());
                int bar = int.Parse(jData[i]["bar"].ToString());
                float beat = float.Parse(jData[i]["beat"].ToString());
                float unitVecX = float.Parse(jData[i]["unitVecX"].ToString());
                float unitVecY = float.Parse(jData[i]["unitVecY"].ToString());
                editorManager.AddNote(xPos, bar, beat, unitVecX, unitVecY);
            }
        }
        else
        {
            // failed to load file.
        }
    }
    
}