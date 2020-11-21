using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music55 : MonoBehaviour
{
    AudioSource audioSource; int n = 0; int j = 0; int k = 0; int i = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void ClickMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/点击2"));
    }

    public void BlankWhritMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/黑白"));
    }
    public void DayLightMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/开日光"));
    }
    public void CloseLightMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/关机2"));
    }
    public void LightMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/灯亮了"));
    }

    //等把imagetarget的脚本写了再来这调用
    public void YiShuMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/灯亮了"));
    }
    public void NvMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/灯亮了"));
    }
    public void ChengXuMusic()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/灯亮了"));
    }
}
