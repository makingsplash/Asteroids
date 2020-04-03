using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private bool _isQuitting = false;
    private AudioSource _audioSource;

    private static AudioController _instance;
    public static AudioController Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<AudioController>();
                if(_instance == null)
                {
                    _instance = new GameObject().AddComponent<AudioController>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        Application.wantsToQuit += OnQuitting;

        if (_instance != null && _instance != this)
            Destroy(gameObject);
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayOneSound(AudioClip sound)
    {
        // Для избежания NullRef обращения к компоненту из-за случайного порядка удаления игровых объектов
        if (!_isQuitting && gameObject.GetComponent<AudioSource>() != null)
                _audioSource.PlayOneShot(sound);
    }

    bool OnQuitting()
    {
        _isQuitting = true;
        return _isQuitting;
    }
}
