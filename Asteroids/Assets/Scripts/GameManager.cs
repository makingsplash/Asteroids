using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private bool _gameOver = false;
    private EnemySpawner _enemySpawner;

    private int _lifes = 3;

    private void Awake()
    {
        _player.SetActive(false);
    }
    private void OnEnable()
    {
        MeteoriteController.OnMeteoriteTouchedPlayer += PlayerWasTouched;
        UFOController.OnUFOTouchedPlayer += PlayerWasTouched;

        LazerController.OnNoMoreEnemies += GameWin;
    }

    private void OnDisable()
    {
        MeteoriteController.OnMeteoriteTouchedPlayer -= PlayerWasTouched;
        UFOController.OnUFOTouchedPlayer -= PlayerWasTouched;

        LazerController.OnNoMoreEnemies -= GameWin;
    }

    private void Start()
    {
        Screen.fullScreen = false;
        _enemySpawner = GetComponent<EnemySpawner>();

        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (_gameOver)
            if (Input.GetKeyDown(KeyCode.R))
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    IEnumerator StartGame()
    {
        StartCoroutine(PlayerRespawn());
        yield return new WaitForSeconds(5);

        StartCoroutine(_enemySpawner.SpawnBigMeteorites());
        StartCoroutine(_enemySpawner.SpawnUFO());
    }

    void PlayerWasTouched()
    {
        // PlayerController
        _player.SetActive(false);
        if (_lifes > 1)
        {
            // PlayerController
            StartCoroutine(PlayerRespawn());

            _lifes--;
            UIManager.Instance.PlayerDead();
        }
        else
        {
            UIManager.Instance.PlayerDead();

            // Game Manager
            _gameOver = true;
        }
    }

    // PlayerController
    IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(2);
        UIManager.Instance.PlayerRespawned();
        _player.SetActive(true);
    }
    
    void GameWin()
    {
        _gameOver = true;
        UIManager.Instance.WinMessage();
    }
}
