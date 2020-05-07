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

            Debug.LogError("There is no SceneManager in the scene");

            return null;
        }
    }

    [SerializeField] private GameObject _rocket;

    private byte _lifesAmount = 3;
    private bool _gameOver = false;
    [SerializeField] private RocketSpawner _rocketSpawner;
    [SerializeField] private WaveManager _waveManager;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("There is more than one SceneManager in the Scene");
            return;
        }
        else if (_instance != null)
            _instance = this;

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Screen.fullScreen = false;
    }

    private void Update()
    {
        if (_gameOver)
            if (Input.GetMouseButtonDown(0))
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void LaunchGame() => StartCoroutine(Launch());

    private IEnumerator Launch()
    {
        yield return StartCoroutine(_rocketSpawner.SpawnRocket());
        StartCoroutine(_waveManager.NextWave());
    }

	public void PlayerDead()
    {
        if (_lifesAmount > 1)
        {
            _lifesAmount--;

            UIManager.Instance.ChangeScore(-40);
            UIManager.Instance.ShowRespawnMessage();

            StartCoroutine(_rocketSpawner.SpawnRocket());
        }
        else
        {
            UIManager.Instance.GameOverMessage();
            GameOver();
        }

        _rocket.SetActive(false);
    }

    public void GameWin()
    {
        _gameOver = true;
        UIManager.Instance.WinMessage();
    }

    void GameOver() => _gameOver = true;
}
