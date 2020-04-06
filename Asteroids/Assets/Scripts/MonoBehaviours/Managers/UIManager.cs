using UnityEngine;
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

    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _currentScore;
    [SerializeField] private GameObject[] _lifesUI = new GameObject[3];

    private int _lifesAmount = 3;

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

    public void DecreaseLifes()
    {
        _lifesUI[--_lifesAmount].SetActive(false);
    }

    public void PlayerDead()
    {
        if (_lifesAmount > 1)
        {
            DecreaseLifes();

            ChangeScore(-40);

            _messageText.text = "Respawning..";
            _messageText.gameObject.SetActive(true);
        }
        else
        {
            _messageText.text = "Game over" + "\n" + "Press R to restart";
            _messageText.gameObject.SetActive(true);
        }
    }

    public void DisableMessage() => _messageText.gameObject.SetActive(false);

    public void WinMessage()
    {
        _messageText.text = "You won";
        _messageText.gameObject.SetActive(true);
    }
}
