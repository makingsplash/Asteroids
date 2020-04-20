using UnityEngine;

public class RocketInput : MonoBehaviour
{
    [HideInInspector] public float Vertical;
    [HideInInspector] public float Horizontal;
    [HideInInspector] public bool IsShotTapDown;

    private bool _right = false;
    private bool _left = false;


    private void Update()
    {
        if (_left && !_right)
            Horizontal = Mathf.Lerp(Horizontal, -1, Time.deltaTime);

        if (!_left && _right)
            Horizontal = Mathf.Lerp(Horizontal, 1, Time.deltaTime);

        if (!_left && !_right || _left && _right)
            Horizontal = Mathf.Lerp(Horizontal, 0, Time.deltaTime * 0.7f);
    }

    public void TapShotDown() => IsShotTapDown = true;
    public void TapShotUp() => IsShotTapDown = false;

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

    public void TapForwardDown() => Vertical = 1;
    public void TapForwardUp() => Vertical = 0;
}
