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
    [SerializeField] private TextMeshProUGUI _messageText;
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
    }

    public void ChangeScore(int points)
    {
        points = Mathf.Clamp((int.Parse(_currentScore.text) + points), 0, int.MaxValue);

        _currentScore.text = points.ToString();
    }

    public void ShowRespawnMessage()
    {
        _messageText.text = "Respawning..";
        _messageText.gameObject.SetActive(true);
    }
    public void DisableMessage() => _messageText.gameObject.SetActive(false);

    public void DecreaseLifes() => _lifesUI[--_lifesAmount].SetActive(false);

    public void WinMessage()
    {
        _messageText.text = "You won";
        _messageText.gameObject.SetActive(true);
    }

    public void GameOverMessage()
    {
        _messageText.text = "Game over" + "\n" + "Press R to restart";
        _messageText.gameObject.SetActive(true);
    }

    public void ChangeWaveCounter(byte waveNumber)
    {
        _waveCounter.text = "Wave: " + waveNumber;
    }

    public void DecreaseWaveBarCurrentValue() => StartCoroutine(_enemyWaveBar.DecreaseCurrentValue());

    public void ResizeWaveBarMaxValue() => _enemyWaveBar.ResizeMaxValue();

    public void DisableShieldButton()
    {
        _shieldButton.image.color = Color.black;
        _shieldButton.interactable = false;
    }

    public void PrepareShieldButton() => _shieldButton.image.color = Color.gray;
    public void EnableShieldButton()
    {
        _shieldButton.image.color = Color.white;
        _shieldButton.interactable = true;
    }

    private IEnumerator ShieldTimer(float time)
    {
        _shieldTimer.text = time.ToString();
        _shieldTimer.gameObject.SetActive(true);

        float oneTick = 0.05f;
        WaitForSeconds wait = new WaitForSeconds(oneTick);
        
        while(time != 0)
        {
            time -= oneTick;
            _shieldTimer.text = time.ToString();
            yield return wait;
        }
        _shieldTimer.gameObject.SetActive(false);
    }
}
