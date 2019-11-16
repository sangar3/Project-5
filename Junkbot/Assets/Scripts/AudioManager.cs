using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Static Instance 
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if(instance == null )
            {
                instance = FindObjectOfType<AudioManager>();
                if(instance == null)
                {
                    instance = new GameObject("Spawned AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();   //cretes audio manaager if its not in the scene
                }

            }

            return instance;
        }
        private set //cannont set the audio manager from any other script;
        {
            instance = value;

        }
    }
    #endregion

    #region fields 
    public AudioSource musicSource;
    public AudioSource sfxSource;


    #endregion

    private void Awake()
    {
        //make sure we dont destory this instance 
       

        // create audio sources, and save them as references
        musicSource = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();

        //loop the music tracks 

        musicSource.loop = true;
        //DontDestroyOnLoad(this.gameObject);
    }



    public void PlayMusic(AudioClip musicClip)
    {
        musicSource.clip = musicClip;
        musicSource.Play(0);

    }

    public void PlaySFX(AudioClip clip, float volume  )
    {
        sfxSource.PlayOneShot(clip, volume);    // no cuts in the sound 
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }


}
