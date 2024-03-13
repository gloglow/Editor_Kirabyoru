using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DataManager dataManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsMusicPlaying()　//　音楽がプレイ中かどうかを返還
    {
        return audioSource.isPlaying;
    }

    public void MusicSelect()　//　音楽を選択
    {
        string fileAddress = dataManager.MusicFileSelect();　//　エクスプローラーを開け、音楽ファイルを探す
        if(fileAddress != null)
        {
            StartCoroutine(ChangeMusic(fileAddress));
        }
    }

    private IEnumerator ChangeMusic(string fileAddress)　//　音楽ファイルを開ける
    {
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

    public bool MusicPlay()　//　音楽を再生
    {
        if (audioSource == null)
            return false;
        audioSource.Play();
        return true;
    }

    public void MusicPause()　//　音楽を一時中止
    {
        audioSource.Pause();
    }

    public void MusicResume()　//　音楽をまた再生
    {
        audioSource.UnPause();
    }

    public void MusicStop()　//　音楽をストップ
    {
        audioSource.Stop();
    }
}