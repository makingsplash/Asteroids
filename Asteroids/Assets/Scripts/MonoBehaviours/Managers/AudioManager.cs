using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if(_instance == null)
                {
                    _instance = new GameObject().AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        if (_instance == null)
        {
            _instance = this;
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayOneSound(AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }
}
