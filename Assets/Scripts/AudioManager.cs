using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    // manage audio play and stop
    private AudioSource audioSource;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DataManager dataManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsMusicPlaying()
    {
        return audioSource.isPlaying;
    }

    public void MusicSelect()
    {
        string fileAddress = dataManager.MusicFileSelect();
        if(fileAddress != null )
        {
            StartCoroutine(ChangeMusic(fileAddress));
        }
    }

    private IEnumerator ChangeMusic(string fileAddress)
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
                uiManager.BackToDefault();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    public bool MusicPlay()
    {
        if (audioSource == null)
            return false;
        audioSource.Play();
        return true;
    }

    public void MusicPause()
    {
        audioSource.Pause();
    }

    public void MusicResume()
    {
        audioSource.UnPause();
    }

    public void MusicStop()
    {
        audioSource.Stop();
    }
}