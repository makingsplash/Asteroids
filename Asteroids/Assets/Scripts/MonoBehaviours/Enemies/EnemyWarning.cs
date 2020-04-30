using System.Collections;
using UnityEngine;

public class EnemyWarning : MonoBehaviour, IPoolObject
{
    [HideInInspector] public ObjectPool ParentPool { get; set; }

    private GameObject _enemyObject;
    [HideInInspector] public GameObject EnemyObject
    {
        set
        {
            _enemyObject = value;
            ActivateWarning();
        }
    }

    private Transform _enemyTransform;
    private CheckCameraVisability _enemyVisability;

    private Transform _transform;

    private float _camOrtSize;
    private float _camAspect;

    private float _warningX;
    private float _warningY;

    private const float _borderOffset = 0.55f;
    private const float _targetScale = 0.15f;
    private const float _startScale = 0.01f;


    private void OnEnable()
    {
        if(_transform == null)
            _transform = GetComponent<Transform>();

        _camOrtSize = CameraInfo.Instance.CamOrtSize;
        _camAspect = CameraInfo.Instance.CamAspect;

        _transform.localScale = Vector3.one * _startScale;
    }

    private void Update()
    {
        _transform.position = new Vector2(_warningX, _warningY);
    }

    private void ActivateWarning()
    {
        PrepareWarning();
        StartCoroutine(ScaleObject());
        StartCoroutine(UpdatePosition());
    }

    private void PrepareWarning()
    {
        _enemyTransform = _enemyObject.GetComponent<Transform>();
        _enemyVisability = _enemyObject.GetComponent<CheckCameraVisability>();
    }

    private IEnumerator ScaleObject()
    {
        Vector3 oneChange = new Vector3(0.0025f, 0.0025f, 0.0025f);
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while(!_enemyVisability.IsVisible)
        {
            if (_transform.localScale.x < _targetScale)
                _transform.localScale += oneChange;
            yield return wait;
        }

        ReturnToPool();
    }

    private IEnumerator UpdatePosition()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (true)
        {
            if (_enemyTransform.position.x > _camOrtSize * _camAspect)
                _warningX = _camOrtSize * _camAspect - _borderOffset;
            else if (_enemyTransform.position.x < -_camOrtSize * _camAspect)
                _warningX = -_camOrtSize * _camAspect + _borderOffset;
            else
                _warningX = _enemyTransform.position.x;

            if (_enemyTransform.position.y > _camOrtSize)
                _warningY = _camOrtSize - _borderOffset;
            else if (_enemyTransform.position.y < -_camOrtSize)
                _warningY = -_camOrtSize + _borderOffset;
            else
                _warningY = _enemyTransform.position.y;

            yield return wait;
        }
    }
    
    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        ParentPool.Pool.Enqueue(gameObject);
    }
}
