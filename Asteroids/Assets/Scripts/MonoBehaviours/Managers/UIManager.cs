using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<UIManager>();

            if (_instance != null)
                return _instance;

            Debug.LogError("There is no UIManager in the scene");
            return null;
        }
    }


    [Header("Playing elements")]
    [SerializeField] private Button _shieldButton;
    [SerializeField] private TextMeshProUGUI _shieldTimer;
    [SerializeField] private GameObject[] _lifesUI = new GameObject[3];
    private byte _lifesAmount = 3;

    [Header("Playing and pause elements")]
    [SerializeField] private EnemyWaveBar _enemyWaveBar;
    [SerializeField] private Button _pauseButton;
    private bool _onPause = false;
    private Color _camDefaultColor;

    [Header("Common elements")]
    [SerializeField] private TextMeshProUGUI _waveCounter;
    [SerializeField] private TextMeshProUGUI _messageBoxText;
    [SerializeField] private TextMeshProUGUI _currentScore;
    private Transform _scoreTransform;
    private Coroutine _scorePulsing;

    [Header("Pages")]
    [SerializeField] private GameObject _menuElements;
    [SerializeField] private GameObject _playingElements;
    [SerializeField] private GameObject _playingPauseElements;
    [SerializeField] private GameObject _pauseElements;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("There is more than one UIManager in the scene");
            return;
        }
        if (_instance == null)
            _instance = this;

        _scoreTransform = _currentScore.gameObject.GetComponent<Transform>();

        ShowMenuElements();

        ChangeWaveCounter(SaveManager.Instance.CurrentWave);
        ChangeScore(SaveManager.Instance.Score);
    }

    public void StartGame()
    {
        SceneManager.Instance.LaunchGame();
        ShowPlayingElements();
    }


	#region Pages
	public void ShowMenuElements()
    {
        _pauseElements.SetActive(false);
        _playingElements.SetActive(false);
        _playingPauseElements.SetActive(false);

        _menuElements.SetActive(true);
    }

    public void ShowPlayingElements()
    {
        _pauseElements.SetActive(false);
        _menuElements.SetActive(false);

        _playingElements.SetActive(true);
        _playingPauseElements.SetActive(true);
    }

    public void ShowPauseElements()
    {
        _playingElements.SetActive(false);

        _pauseElements.SetActive(true);
    }
    #endregion

    public void DecreaseLifes() => _lifesUI[--_lifesAmount].SetActive(false);

	#region Pause
	public void UsePauseButton()
    {
        if (_onPause)
            UnPauseGame();
        else
            PauseGame();
    }
    public void HomeButton()
    {
        SaveManager.Instance.Score = int.Parse(_currentScore.text);
        SaveManager.Instance.CurrentWave -= 2;
        UnPauseGame();

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        ShowMenuElements();
    }

    private void PauseGame()
    {
        ShowPauseElements();

        _pauseButton.image.sprite = _pauseButton.spriteState.disabledSprite;

        _messageBoxText.text = "Pause";
        _messageBoxText.gameObject.SetActive(true);

        _camDefaultColor = Camera.main.backgroundColor;
        Camera.main.backgroundColor = Color.gray;

        _onPause = true;
        Time.timeScale = 0;
    }

    private void UnPauseGame()
    {
        ShowPlayingElements();

        _pauseButton.image.sprite = _pauseButton.spriteState.pressedSprite;

        _messageBoxText.gameObject.SetActive(false);

        Camera.main.backgroundColor = _camDefaultColor;

        _onPause = false;
        Time.timeScale = 1;
    }
    #endregion

    #region Score
	public void ChangeScore(int points)
    {
        int currentScore = int.Parse(_currentScore.text);
        if(points > 0)
        {
            if (_scorePulsing != null)
                StopCoroutine(_scorePulsing);
            _scorePulsing = StartCoroutine(MakeScorePulse());
        }
        points = currentScore + points > 0 ? currentScore + points : 0;
        _currentScore.text = points.ToString();

        SaveManager.Instance.Score = currentScore;
    }

    private IEnumerator MakeScorePulse()
    {
        float _startScale = 1;
        float _targetScale = 1.2f;
        Vector3 oneScaleChange = Vector3.one * 0.08f;

        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (_scoreTransform.localScale.x < _targetScale)
        {
            _scoreTransform.localScale += oneScaleChange;
            yield return wait;
        }

        while (_scoreTransform.localScale.x > _startScale)
        {
            _scoreTransform.localScale -= oneScaleChange;
            yield return wait;
        }
    }
    #endregion

	#region MessageBox
	public void ShowRespawnMessage()
    {
        _messageBoxText.text = "Respawning..";
        _messageBoxText.gameObject.SetActive(true);
    }
    public void HideMessageBox() => _messageBoxText.gameObject.SetActive(false);

    public void WinMessage()
    {
        _messageBoxText.text = "You won";
        _messageBoxText.gameObject.SetActive(true);
    }

    public void GameOverMessage()
    {
        _messageBoxText.text = "Game over" + "\n" + "Press R to restart";
        _messageBoxText.gameObject.SetActive(true);
    }
    #endregion

    #region Enemy wave bar

    public void ChangeWaveCounter(int waveNumber)
    {
        _waveCounter.text = "Wave: " + (waveNumber + 1);
    }

    public void DecreaseWaveBarCurrentValue() => StartCoroutine(_enemyWaveBar.DecreaseCurrentValue());
    
    public void ResizeWaveBarMaxValue() => _enemyWaveBar.ResizeMaxValue();
    #endregion

    #region Shield button

    public IEnumerator DisableShieldButton(float disableTimer)
    {
        StopAllCoroutines();
        _shieldButton.image.color = Color.black;
        _shieldButton.interactable = false;
        yield return StartCoroutine(StartShieldTimer(disableTimer));
    }

    public IEnumerator PrepareShieldButton(float reloadTimer)
    {
        _shieldButton.image.color = Color.gray;
        yield return StartCoroutine(StartShieldTimer(reloadTimer));
    }

    public void EnableShieldButton()
    {
        _shieldButton.image.color = Color.white;
        _shieldButton.interactable = true;
    }

    private IEnumerator StartShieldTimer(float time)
    {
        _shieldTimer.gameObject.SetActive(true);

        float oneTick = 0.01f;
        WaitForSeconds wait = new WaitForSeconds(oneTick);

        while(time > 0)
        {
            _shieldTimer.text = time.ToString("f2");
            yield return wait;
            time -= oneTick;
        }
        _shieldTimer.gameObject.SetActive(false);
    }
    #endregion
}
