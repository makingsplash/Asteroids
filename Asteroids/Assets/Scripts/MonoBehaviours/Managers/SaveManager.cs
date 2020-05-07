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

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            PlayerPrefs.DeleteAll();
            Debug.LogError("Сохранения сброшены");
        }
    }

    public void SaveCurrentWaveNumber(byte waveNumber) => PlayerPrefs.SetInt("CurrentWave", waveNumber);
    public byte GetCurrentWaveNumber() => (byte) PlayerPrefs.GetInt("CurrentWave"); 

}
