using System;
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

    [Header("Top elements")]
    [SerializeField] private TextMeshProUGUI _messageBoxText;
    [SerializeField] private TextMeshProUGUI _currentScore;
    [SerializeField] private EnemyWaveBar _enemyWaveBar;
    [SerializeField] private TextMeshProUGUI _waveCounter;

    [Header("Rocket lifes")]
    [SerializeField] private GameObject[] _lifesUI = new GameObject[3];

    [Header("Shield button")]
    [SerializeField] private Button _shieldButton;
    [SerializeField] private TextMeshProUGUI _shieldTimer;

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
    }

    public void ChangeScore(int points)
    {
        points = Mathf.Clamp((int.Parse(_currentScore.text) + points), 0, int.MaxValue);

        _currentScore.text = points.ToString();
    }
    public void DecreaseLifes() => _lifesUI[--_lifesAmount].SetActive(false);

	#region MessageBox
	public void ShowRespawnMessage()
    {
        _messageBoxText.text = "Respawning..";
        _messageBoxText.gameObject.SetActive(true);
    }
    public void DisableMessage() => _messageBoxText.gameObject.SetActive(false);

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
