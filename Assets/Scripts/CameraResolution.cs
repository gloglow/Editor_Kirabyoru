using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    //　画面比率が変わってもオブジェクトやUIの位置を同じにする

    //　理想的な画面比率。1920x1080で設定。
    [SerializeField] private float idealScreenWidth;
    [SerializeField] private float idealScreenHeight;

    //　理想的な画面比率を基準とし、
    //　縦　＜　横　：　true
    //　縦　＞　横　：　false
    public bool isWideThanIdeal;

    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;

        float screenScale = ((float)Screen.width / Screen.height) / (idealScreenWidth / idealScreenHeight);　//　現在の画面比率と理想的な画面比率を比較
        
        if(screenScale < 1) // 縦　＞　横
        {
            rect.height = screenScale;
            rect.y = (1f - screenScale) / 2f;
            isWideThanIdeal = false;
        }
        else // 縦　＜　横
        {
            float scaleWidth = 1f / screenScale;
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
            isWideThanIdeal = true;
        }
        camera.rect = rect;
    }
}
