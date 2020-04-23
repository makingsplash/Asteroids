using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private readonly float _oneChange = 0.025f;
    private readonly WaitForSeconds _wait = new WaitForSeconds(0.01f);


    public void ResizeMaxValue(byte health)
    {
        _slider.maxValue = health;
        _slider.value = health;
    }

    public IEnumerator DecreaseCurrentValue()
    {
        float nextValue = _slider.value - 1;
        while (_slider.value > nextValue)
        {
            _slider.value -= _oneChange;
            yield return _wait;
        }
    }
}
