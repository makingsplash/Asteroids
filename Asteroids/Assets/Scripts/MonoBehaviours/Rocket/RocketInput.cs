using UnityEngine;

public class RocketInput : MonoBehaviour
{
    [HideInInspector] public float Vertical;
    [HideInInspector] public float Horizontal;
    [HideInInspector] public bool IsShotTapDown;

    private bool _right = false;
    private bool _left = false;
    private RocketController _rocketController;

    private void OnEnable()
    {
        _rocketController = GetComponent<RocketController>();
    }

    private void Update()
    {
        if (_left && !_right)
            Horizontal = Mathf.Lerp(Horizontal, -1, Time.deltaTime * 1.3f);

        if (!_left && _right)
            Horizontal = Mathf.Lerp(Horizontal, 1, Time.deltaTime * 1.3f);

        if (!_left && !_right || _left && _right)
            Horizontal = Mathf.Lerp(Horizontal, 0, Time.deltaTime * 1.5f);
    }

    public void TapShotDown() => IsShotTapDown = true;
    public void TapShotUp() => IsShotTapDown = false;
    public void TapForwardDown() => Vertical = 1;
    public void TapForwardUp() => Vertical = 0;

    public void TapLeftDown()
    {
        _left = true;
        _right = false;
    }
    public void TapLeftUp()
    {
        _left = false;
        _right = false;
    }
    public void TapRightDown()
    {
        _left = false;
        _right = true;
    }
    public void TapRightUp()
    {
        _left = false;
        _right = false;
    }

    public void TapShieldDown()
    {
        if(_rocketController.gameObject.activeSelf)
            StartCoroutine(_rocketController.UseShield());
    }
}
