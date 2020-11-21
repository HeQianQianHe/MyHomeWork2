using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music5 : MonoBehaviour
{

    AudioSource audioSource; int n = 0; int j = 0; int k = 0; int i = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }



    void Update()
    {
        if (audioSource.isPlaying==false)
        {
            ChuXianMusic();
        }
    }
    public void ChuXianMusic()
    {
        if (n == 1)
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/收音机1"),0.6f);
        }
        else if (n == 0)
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/收音机2"),0.6f);
        }
        else if (n == 2)
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/收音机3"), 0.6f);
        }
        else if (n == 3)
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/收音机4"), 0.6f);
        }
        else if (n == 4)
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Music/收音机5"), 0.6f);
        }
        n++;
        if (n == 5)
        {
            n = 0;
        }
    }
}
