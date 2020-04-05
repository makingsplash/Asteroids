using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
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
    private bool _gameOver = false;
    private EnemySpawner _enemySpawner;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("There is more than one SceneManager in the Scene");
            return;
        }
        else if (_instance != null)
            _instance = this;


        _enemySpawner = GetComponentInChildren<EnemySpawner>();
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
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    IEnumerator StartGame()
    {
        StartCoroutine(SpawnPlayer());
        yield return new WaitForSeconds(3);

        StartEnemyWaveSpawning();
    }

    private void StartEnemyWaveSpawning() => _enemySpawner.NextWave();

	public void PlayerDead()
    {
        if (_lifesAmount > 1)
        {
            _lifesAmount--;

            StartCoroutine(SpawnPlayer());
        }
        else
        {
            GameOver();
        }

        _player.SetActive(false);

        UIManager.Instance.PlayerDead();
    }

    public IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(2);
        UIManager.Instance.DisableMessage();
        _player.SetActive(true);
    }

    public void GameWin()
    {
        _gameOver = true;
        UIManager.Instance.WinMessage();
    }

    void GameOver() => _gameOver = true;
}
