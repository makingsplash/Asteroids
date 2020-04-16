using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInfo : MonoBehaviour
{
    private static CameraInfo _instance;
    public static CameraInfo Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<CameraInfo>();
            if (_instance != null)
                return _instance;

            Debug.LogError("There is no CameraInfo in the scene");

            return null;
        }
    }

    public float CamOrtSize;
    public float CamAspect;


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Debug.LogError("There is more than 1 CameraInfo in the scene");
        else if (_instance == null)
            _instance = this;

        CamOrtSize = Camera.main.orthographicSize;
        CamAspect = Camera.main.aspect;
    }
}
