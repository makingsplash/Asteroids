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

    private void Awake()
    {
        _player.SetActive(false);
    }
    private void OnEnable()
    {
        BaseEnemy.OnNoOtherEnemies += GameWin;

        SceneManager.OnNoLifes += GameOver;
    }

    private void OnDisable()
    {
        BaseEnemy.OnNoOtherEnemies -= GameWin;

        SceneManager.OnNoLifes -= GameOver;
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
        StartCoroutine(SceneManager.Instance.RespawnPlayer());
        yield return new WaitForSeconds(5);

        StartCoroutine(_enemySpawner.SpawnBigMeteorites());
        StartCoroutine(_enemySpawner.SpawnUFO());
    }
    
    void GameWin()
    {
        _gameOver = true;
        UIManager.Instance.WinMessage();
    }

    void GameOver() => _gameOver = true;
}
