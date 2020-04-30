using System.Collections;
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
    private const float _targetScale = 0.75f;
    private const float _startScale = 0.2f;


    private void OnEnable()
    {
        if(_warningTransform == null)
            _warningTransform = WarningObject.GetComponent<Transform>();

        if(_transform == null)
            _transform = GetComponent<Transform>();

        _camOrtSize = CameraInfo.Instance.CamOrtSize;
        _camAspect = CameraInfo.Instance.CamAspect;

        StartCoroutine(ScaleObject());
    }

    private void Update()
    {
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

    private IEnumerator ScaleObject()
    {
        _warningTransform.localScale = Vector3.one * _startScale;

        Vector3 oneChange = new Vector3(0.01f, 0.01f, 0.01f);

        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while(_warningTransform.localScale.x < _targetScale)
        {
            _warningTransform.localScale += oneChange;
            yield return wait;
        }
    }

}
