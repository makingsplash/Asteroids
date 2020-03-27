using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _gameOver = false;

    
    private void OnEnable()
    {

        LazerController.OnNoMoreEnemies += GameWin;

        SceneManager.OnNoLifes += GameOver;
    }

    private void OnDisable()
    {

        LazerController.OnNoMoreEnemies -= GameWin;

        SceneManager.OnNoLifes -= GameOver;
    }

    private void Start()
    {
        Screen.fullScreen = false;

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

        SceneManager.Instance.StartEnemySpawner();
    }
    
    void GameWin()
    {
        _gameOver = true;
        UIManager.Instance.WinMessage();
    }

    void GameOver() => _gameOver = true;
}
