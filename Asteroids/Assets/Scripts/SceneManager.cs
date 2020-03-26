using System;
using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static Action OnNoLifes;

    private static SceneManager _instance;
    public static SceneManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<SceneManager>();

            if (_instance != null)
                return _instance;

            Debug.LogError("There is on SceneManager in the scene");

            return null;
        }
    }

    [SerializeField] private GameObject _player;

    private int _lifesAmount = 3;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("There is more than one SceneManager in the Scene");
            return;
        }
        else if (_instance != null)
            _instance = this;
    }


    public void PlayerDead()
    {
        if (_lifesAmount > 1)
        {
            _lifesAmount--;

            StartCoroutine(RespawnPlayer());
        }
        else
        {
            OnNoLifes();
        }

        _player.SetActive(false);

        UIManager.Instance.PlayerDead();
    }
    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2);
        UIManager.Instance.PlayerRespawned();
        _player.SetActive(true);
    }
}
