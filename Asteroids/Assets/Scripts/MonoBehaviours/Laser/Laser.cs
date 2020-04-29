using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour, IDamager, IPoolObject
{
    public ObjectPool ParentPool { get; set; }

    [SerializeField] private byte _damage;
    [SerializeField] private ushort _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private bool _needToCheckVisability;

    private float _currentLifeTime;
    private CheckCameraVisability _checkVisability;


    private void OnEnable()
    {
        _currentLifeTime = _lifeTime;

        _checkVisability = GetComponent<CheckCameraVisability>();
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime < 0 || _needToCheckVisability && !_checkVisability.IsVisible)
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
        ParentPool.Pool.Enqueue(gameObject);
        gameObject.SetActive(false);
    }
}