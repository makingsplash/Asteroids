using UnityEngine;

public class EnemyWarning : MonoBehaviour
{
    public GameObject WarningObject;

    private float _camOrtSize;
    private float _camAspect;
    private Transform _warningTransform;
    private Transform _transform;
    private float _warningX;
    private float _warningY;
    private const float _borderOffset = 0.2f;
    //private Sprite _warningSprite;


    private void OnEnable()
    {
        _warningTransform = WarningObject.GetComponent<Transform>();
        _transform = GetComponent<Transform>();

        _camOrtSize = CameraInfo.Instance.CamOrtSize;
        _camAspect = CameraInfo.Instance.CamAspect;
    }

    private void Update()
    {
        // update scale

        // update position
        if (_transform.position.x > _camOrtSize * _camAspect)
            _warningX = _camOrtSize * _camAspect - _borderOffset;
        else if (_transform.position.x < -_camOrtSize * _camAspect)
            _warningX = -_camOrtSize * _camAspect + _borderOffset;
        else
            _warningX = transform.position.x;

        if (_transform.position.y > _camOrtSize)
            _warningY = _camOrtSize - _borderOffset;
        else if (_transform.position.y < -_camOrtSize)
            _warningY = -_camOrtSize + _borderOffset;
        else
            _warningY = transform.position.y;

        _warningTransform.position = new Vector2(_warningX, _warningY);
    }

    // IsVisibleInCamera тот скрипт не нужен получается?

}
