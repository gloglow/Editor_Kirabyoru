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
    [SerializeField] private EditorManager editorManager;
    [SerializeField] private UIManager uiManager;

    private VistaOpenFileDialog openDialog;
    private VistaSaveFileDialog saveDialog;
    private Stream openStream = null;

    private void Start()
    {
        openDialog = new VistaOpenFileDialog();
        saveDialog = new VistaSaveFileDialog();
    }

    public string MusicFileSelect()　//　音楽ファイルを選択
    {
        openDialog.Filter = "wav files (*.wav)|*.wav|mp3 files (*.mp3)|*.mp3";
        openDialog.FilterIndex = 2;
        openDialog.Title = "Music File Dialog";
        string fileAddress = FileOpen();

        if(fileAddress != null)
        {
            // 音楽タイトルを分かるため、ファイルのアドレスをファイルの名前に変換
            Regex regex = new Regex(@"\..*$");
            uiManager.ChangeSongText(regex.Replace(Path.GetFileName(fileAddress), ""));
        }
        return fileAddress;
    }

    private string JsonFileSelect()　//　譜面ファイルを選択
    {
        openDialog.Filter = "json files (*.json)|*.json";
        openDialog.FilterIndex = 1;
        openDialog.Title = "json File Dialog";
        return FileOpen();
    }

    private string FileOpen()　//　エクスプローラーを開ける
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

    public void FileSave(List<NoteData> noteList)　//　ノーツのリストを譜面ファイルに変換
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
    
    public void SheetSelect()　// ロードボタンを押すと、譜面ファイルを選択、それをノーツのリストに変換してeditor managerに渡す
    {
        LoadSheetFile(JsonFileSelect());
    }

    private void LoadSheetFile(string fileAddress)　// 譜面ファイルを読み込み、ノーツを追加させる
    {
        if (File.Exists(fileAddress))
        {
            //　譜面ファイルのデータを読み込む
            var fileText = File.ReadAllText(fileAddress);
            JsonData jData = JsonMapper.ToObject(fileText.ToString());
            
            //　エディター設定を初期化
            editorManager.Initialize();
            uiManager.InitiallizeNoteInfo();
            
            //　ノーツデータの通りにノーツを一つずつ追加
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