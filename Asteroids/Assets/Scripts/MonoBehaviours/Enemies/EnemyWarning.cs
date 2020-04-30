using System.Collections;
using UnityEngine;

public class EnemyWarning : MonoBehaviour
{
    [SerializeField] private Transform _enemyTransform;
    [SerializeField] private CheckCameraVisability _enemyVisability;

    private float _camOrtSize;
    private float _camAspect;

    private Transform _transform;

    private float _warningX;
    private float _warningY;

    private const float _borderOffset = 0.35f;
    private const float _targetScale = 0.8f;
    private const float _startScale = 0.2f;


    private void OnEnable()
    {
        if(_transform == null)
            _transform = GetComponent<Transform>();

        _camOrtSize = CameraInfo.Instance.CamOrtSize;
        _camAspect = CameraInfo.Instance.CamAspect;

        _transform.localScale = Vector3.one * 0.001f;

        StartCoroutine(ScaleObject());
    }

    private void Update()
    {
        // update position
        if (_enemyTransform.position.x > _camOrtSize * _camAspect)
            _warningX = _camOrtSize * _camAspect - _borderOffset;
        else if (_enemyTransform.position.x < -_camOrtSize * _camAspect)
            _warningX = -_camOrtSize * _camAspect + _borderOffset;
        else
            _warningX = transform.position.x;

        if (_enemyTransform.position.y > _camOrtSize)
            _warningY = _camOrtSize - _borderOffset;
        else if (_enemyTransform.position.y < -_camOrtSize)
            _warningY = -_camOrtSize + _borderOffset;
        else
            _warningY = transform.position.y;

        _transform.position = new Vector2(_warningX, _warningY);
    }

    private IEnumerator ScaleObject()
    {
        _transform.localScale = Vector3.one * _startScale;

        Vector3 oneChange = new Vector3(0.05f, 0.05f, 0.05f);

        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while(!_enemyVisability.IsVisible)
        {
            if (_transform.localScale.x < _targetScale)
                _transform.localScale += oneChange;
            yield return wait;
        }

        gameObject.SetActive(false);
    }
}
