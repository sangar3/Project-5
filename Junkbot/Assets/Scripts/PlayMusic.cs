using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public AudioClip music;

    // Update is called once per frame
    void Start()
    {
        AudioManager.Instance.PlayMusic(music);
    }
}
