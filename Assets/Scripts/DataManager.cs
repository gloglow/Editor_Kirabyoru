using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using LitJson;
using Ookii.Dialogs;
using System.Text.RegularExpressions;

public class DataManager : MonoBehaviour
{
    // managers
    [SerializeField] private EditorManager editorManager;
    [SerializeField] private UIManager uiManager;

    // for open files
    private VistaOpenFileDialog openDialog;
    private VistaSaveFileDialog saveDialog;
    private Stream openStream = null;

    private void Start()
    {
        openDialog = new VistaOpenFileDialog();
        saveDialog = new VistaSaveFileDialog();
    }

    public string MusicFileSelect() // select file and return its address
    {
        // open file selecting screen
        openDialog.Filter = "wav files (*.wav)|*.wav|mp3 files (*.mp3)|*.mp3";
        openDialog.FilterIndex = 2;
        openDialog.Title = "Music File Dialog";
        string fileAddress = FileOpen();

        // file address -> file name
        Regex regex = new Regex(@"\..*$");
        uiManager.ChangeSongText(regex.Replace(Path.GetFileName(fileAddress), ""));
        
        return fileAddress;
    }

    private string JsonFileSelect() // select file and return its address
    {
        // open file selecting screen
        openDialog.Filter = "json files (*.json)|*.json";
        openDialog.FilterIndex = 1;
        openDialog.Title = "json File Dialog";
        return FileOpen();
    }

    private string FileOpen() // open file dialog
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

    public void FileSave(List<NoteData> noteList) // get noteList and transform to json file.
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
    
    public void SheetSelect() // if load button pressed, call this method
    {
        LoadSheetFile(JsonFileSelect());
    }

    private void LoadSheetFile(string fileAddress) // Load JSON file having Note information.
    {
        if (File.Exists(fileAddress))
        {
            // load json file.
            var fileText = File.ReadAllText(fileAddress);
            JsonData jData = JsonMapper.ToObject(fileText.ToString());
            
            // initialize setting.
            editorManager.Initialize();
            uiManager.InitiallizeNoteInfo();
            
            // add notes from json data.
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
            return;
        }
    }
    
}