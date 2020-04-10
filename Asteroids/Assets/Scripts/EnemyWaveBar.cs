using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private Coroutine _resizeMaxBarValue;
    private Coroutine _resizeCurrentBarValue;


    public void SetMaxValue()
    {
        if (_resizeMaxBarValue == null)
            _resizeMaxBarValue = StartCoroutine(ResizeMaxBarValue());
    }

    public void SetCurrentValue()
    {
        if (_resizeCurrentBarValue == null)
            _resizeCurrentBarValue = StartCoroutine(ResizeCurrentBarValue());
    }

    private IEnumerator ResizeCurrentBarValue()
    {
        Debug.Log("Started resize max bar coroutine");

        WaitForSeconds wait = new WaitForSeconds(0.01f);
        while(_slider.value >= EnemySpawner.EnemiesSpawned - EnemySpawner.EnemiesKilled)
        {
            _slider.value -= 0.1f;
            yield return wait;
        }
    }

    private IEnumerator ResizeMaxBarValue()
    {
        Debug.Log("Started resize current value bar coroutine");

        WaitForSeconds wait = new WaitForSeconds(0.01f);
        while (_slider.maxValue <= EnemySpawner.EnemiesSpawned)
        {
            _slider.maxValue += 0.1f;
            yield return wait;
        }
        _slider.value = EnemySpawner.EnemiesSpawned - EnemySpawner.EnemiesKilled;
    }
}
