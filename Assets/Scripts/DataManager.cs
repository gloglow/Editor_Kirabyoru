using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class DataManager : MonoBehaviour
{
    private string noteDataFilePath = Application.streamingAssetsPath;

    public void MakeSheetFile(string fileName, NoteData[] noteList) // Save Note information of the song into JSON file.
    {
        
    }

    public List<NoteData> LoadSheetFile(string fileName) // Load JSON file having Note information.
    {
        string fileAddress = noteDataFilePath + fileName + ".json";
        string songTitle;
        int noteCnt;
        List<NoteData> noteList = new List<NoteData>();

        if(File.Exists(fileAddress)) 
        {
            var fileText = File.ReadAllText(fileAddress);
            JsonData jData = JsonMapper.ToObject(fileText.ToString());

            songTitle = jData[0].ToString();
            
            for (int i = 0; i < jData["Notes"].Count; i++)
            {
                NoteData noteData = new NoteData();
                noteData.xPos = float.Parse(jData[1][i]["xPos"].ToString());
                noteData.beat = float.Parse(jData[1][i]["beat"].ToString());
                noteData.unitVecX = float.Parse(jData[1][i]["unitVecX"].ToString());
                noteData.unitVecY = float.Parse(jData[1][i]["unitVecY"].ToString());
                noteList.Add(noteData);
            }
        }
        else
        {
            // failed to load file.
        }
        return noteList;
    }
}