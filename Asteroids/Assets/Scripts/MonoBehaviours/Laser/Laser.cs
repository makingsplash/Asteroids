using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour, IDamager, IPoolObject
{
    public ObjectPool ParentPool { get; set; }

    [SerializeField] private byte _damage;
    [SerializeField] private ushort _speed;
    [SerializeField] private float _lifeTime;
    private float _currentLifeTime;
    private float _camHeigth;
    private float _camWidth;


    private void OnEnable()
    {
        _currentLifeTime = _lifeTime;

        _camHeigth = CameraInfo.Instance.CamOrtSize;
        _camWidth = CameraInfo.Instance.CamAspect * _camHeigth;

        StartCoroutine(CheckCameraBorders());
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime < 0)
            ReturnToPool();

        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DoDamage(collision.gameObject.GetComponent<IDamageable>());
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage(_damage);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        ParentPool.Pool.Enqueue(gameObject);
    }

    private IEnumerator CheckCameraBorders()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            float posX = Mathf.Abs(transform.position.x);
            float posY = Mathf.Abs(transform.position.y);

            if (posX > _camWidth || posY > _camHeigth)
            {
                ReturnToPool();
                break;
            }

            yield return wait;
        }
    }
}