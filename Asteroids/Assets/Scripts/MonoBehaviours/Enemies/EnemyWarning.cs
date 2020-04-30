using System.Collections;
using UnityEngine;

public class EnemyWarning : MonoBehaviour, IPoolObject
{
    [HideInInspector] public ObjectPool ParentPool { get; set; }
    public GameObject EnemyObject;

    private Transform _enemyTransform;
    private CheckCameraVisability _enemyVisability;

    private Transform _transform;

    private float _camOrtSize;
    private float _camAspect;

    private float _warningX;
    private float _warningY;

    private const float _borderOffset = 0.4f;
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
        StartCoroutine(CorrectRotation());
    }

    private void Update()
    {
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
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        yield return wait;

        _enemyTransform = EnemyObject.GetComponent<Transform>();
        _enemyVisability = EnemyObject.GetComponent<CheckCameraVisability>();

        _transform.localScale = Vector3.one * _startScale;

        Vector3 oneChange = new Vector3(0.05f, 0.05f, 0.05f);


        while(!_enemyVisability.IsVisible)
        {
            if (_transform.localScale.x < _targetScale)
                _transform.localScale += oneChange;
            yield return wait;
        }

        ReturnToPool();
    }

    private IEnumerator CorrectRotation()
    {
        yield return new WaitForEndOfFrame();
        _transform.rotation = Quaternion.identity;
    }
    
    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        ParentPool.Pool.Enqueue(gameObject);
    }
}
