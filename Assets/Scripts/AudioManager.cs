using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    // manage audio play and stop
    public AudioSource audioSource;
    public UIManager uiManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void MusicSelect()
    {
        StartCoroutine(ChangeMusic(uiManager.MusicFileSelect()));
    }

    public IEnumerator ChangeMusic(string fileAddress)
    {
        //change music and play. (if fail to find music, music stop.)
        using(UnityWebRequest www = 
            UnityWebRequestMultimedia.GetAudioClip(fileAddress, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            if(www.error == null)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = audioClip;
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    public void MusicPlay()
    {
        audioSource.Play();
    }

    public void MusicPause()
    {
        audioSource.Pause();
    }

    public void MusicStop()
    {
        audioSource.Stop();
    }
}
