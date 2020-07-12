using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public struct SoundStruct
    {
        public string name;
        [Range(0,1)] public float volume;
        [Range(0,2)] public float pitch;
        public AudioClip clip;
    }
    
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<AudioManager>();
            return instance;
        }
    }
    
    public List<SoundStruct> sounds = new List<SoundStruct>();
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(string name)
    {
        var sound = sounds.First(ss => ss.name == name);
        _audioSource.volume = sound.volume;
        _audioSource.pitch = sound.pitch;
        _audioSource.PlayOneShot(sound.clip);
    }
}
