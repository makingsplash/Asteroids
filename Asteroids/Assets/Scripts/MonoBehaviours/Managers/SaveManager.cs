using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<SaveManager>();

            if (_instance != null)
                return _instance;

            Debug.LogError("There is no SceneManager in the scene");

            return null;
        }
    }

    public int Score
    {
        get
        {
            return PlayerPrefs.GetInt("Score");
        }
        set
        {
            PlayerPrefs.SetInt("Score", value);
        }
    }

    public int CurrentWave
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentWave");
        }
        set
        {
            if (value >= 0)
                PlayerPrefs.SetInt("CurrentWave", value);
            else
                PlayerPrefs.SetInt("CurrentWave", 0);
        }
    }

    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Сохранения сброшены");
    }

    ///////// SAVE TESTS
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            DeleteSaves();
        }
    }
}
