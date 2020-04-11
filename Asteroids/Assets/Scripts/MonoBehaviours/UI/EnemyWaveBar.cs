using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private readonly float _oneChange = 0.025f;
    private readonly WaitForSeconds _wait = new WaitForSeconds(0.01f);


    public void ResizeMaxValue()
    {
        _slider.maxValue = EnemySpawner.EnemiesSpawned;

        StartCoroutine(IncreaseCurrentValue());
    }

    public IEnumerator DecreaseCurrentValue()
    {
        while(_slider.value > EnemySpawner.EnemiesSpawned - EnemySpawner.EnemiesKilled)
        {
            _slider.value -= _oneChange;
            yield return _wait;
        }
    }

    private IEnumerator IncreaseCurrentValue()
    {
        while (_slider.value < EnemySpawner.EnemiesSpawned - EnemySpawner.EnemiesKilled)
        {
            _slider.value += _oneChange;
            yield return _wait;
        }
    }
}
