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

    // UI
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _currentScore;
    [SerializeField] private GameObject[] _lifesUI = new GameObject[3];
    private int _lifes = 3;

    private void Awake()
    {
        _currentScore.text = "0";
        _player.SetActive(false);
    }
    private void OnEnable()
    {
        MeteoriteController.OnMeteoriteTouchedPlayer += PlayerWasTouched;
        UFOController.OnUFOTouchedPlayer += PlayerWasTouched;

        MeteoriteController.OnMeteoriteWasShooted += ChangeScore;
        UFOController.OnUFOWasShooted += ChangeScore;

        LazerController.OnNoMoreEnemies += GameWin;
    }

    private void OnDisable()
    {
        MeteoriteController.OnMeteoriteTouchedPlayer -= PlayerWasTouched;
        UFOController.OnUFOTouchedPlayer -= PlayerWasTouched;

        MeteoriteController.OnMeteoriteWasShooted -= ChangeScore;
        UFOController.OnUFOWasShooted -= ChangeScore;

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

    void ChangeScore(int score)
    {
        int changing = Mathf.Clamp((int.Parse(_currentScore.text) + score), 0, int.MaxValue);
        _currentScore.text = changing.ToString();
    }

    void PlayerWasTouched()
    {
        ChangeScore(-40);
        _messageText.text = "Respawning..";
        _messageText.gameObject.SetActive(true);

        _player.SetActive(false);
        _lifes--;
        _lifesUI[_lifes].SetActive(false);
        if (_lifes > 0)
            StartCoroutine(PlayerRespawn());
        else
        {
            _messageText.text = "Game over" + "\n" + "Press R to restart";
            _messageText.gameObject.SetActive(true);
            _gameOver = true;
        }
    }

    IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(2);
        _messageText.gameObject.SetActive(false);
        _player.SetActive(true);
    }
    
    void GameWin()
    {
        _gameOver = true;
        _messageText.text = "You won";
        _messageText.gameObject.SetActive(true);
    }
}
