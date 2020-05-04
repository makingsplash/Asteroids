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

    [Header("Upper elements")]
    [SerializeField] private EnemyWaveBar _enemyWaveBar;
    [SerializeField] private TextMeshProUGUI _waveCounter;

    [SerializeField] private TextMeshProUGUI _currentScore;
    private Transform _scoreTransform;
    private Coroutine _scorePulsing;
    
    [Header("Middle elements")]
    [SerializeField] private TextMeshProUGUI _messageBoxText;
    [SerializeField] private GameObject _playButton;

    [Header("Lower elements")]
    [SerializeField] private Button _shieldButton;
    [SerializeField] private TextMeshProUGUI _shieldTimer;

    [SerializeField] private GameObject _rocketControllerButtons;

    [SerializeField] private GameObject[] _lifesUI = new GameObject[3];
    private byte _lifesAmount = 3;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("There is more than one UIManager in the scene");
            return;
        }
        if (_instance == null)
            _instance = this;

        _shieldTimer.gameObject.SetActive(false);
        _scoreTransform = _currentScore.gameObject.GetComponent<Transform>();

        // change later to one func with list cycle
        HideWaveInfo();
        HideRocketControllerButtons();
        HideRocketLifes();
    }

    public void StartGame()
    {
        Debug.Log("Чё то там не стартует");
        StartCoroutine(SceneManager.Instance.StartGame());

        _playButton.SetActive(false);
        HideMessageBox();
        ShowRocketControllerButtons();
        ShowWaveInfo();
        ShowRocketLifes();
    }

    #region RocketLifes
    private void ShowRocketLifes()
    {
        foreach (GameObject life in _lifesUI)
            life.SetActive(true);
    }

    private void HideRocketLifes()
    {
        foreach (GameObject life in _lifesUI)
            life.SetActive(false);
    }

    public void DecreaseLifes() => _lifesUI[--_lifesAmount].SetActive(false);
    #endregion

    #region RocketController buttons
    private void ShowRocketControllerButtons() => _rocketControllerButtons.SetActive(true);

    private void HideRocketControllerButtons() => _rocketControllerButtons.SetActive(false);
    #endregion

    #region Score
    private void ShowScore() => _currentScore.gameObject.SetActive(true);
    private void HideScore() => _currentScore.gameObject.SetActive(false);

	public void ChangeScore(int points)
    {
        int currentScore = int.Parse(_currentScore.text);
        points = currentScore + points > 0 ? currentScore + points : 0;
        if(points > 0)
        {
            if (_scorePulsing != null)
                StopCoroutine(_scorePulsing);
            _scorePulsing = StartCoroutine(MakeScorePulse());
        }
        _currentScore.text = points.ToString();
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
    private void ShowWaveInfo()
    {
        _waveCounter.gameObject.SetActive(true);
        _enemyWaveBar.gameObject.SetActive(true);
    }

    private void HideWaveInfo()
    {
        _waveCounter.gameObject.SetActive(false);
        _enemyWaveBar.gameObject.SetActive(false);
    }

    public void ChangeWaveCounter(byte waveNumber) => _waveCounter.text = "Wave: " + waveNumber;

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
