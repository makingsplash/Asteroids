using UnityEngine;

public class Laser : MonoBehaviour, IDamager, IPoolObject
{
    public ObjectPool ParentPool { get; set; }

    [SerializeField] private byte _damage;
    [SerializeField] private ushort _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private bool _checkVisability;

    private float _currentLifeTime;
    private CheckCameraVisability _visability;


    private void OnEnable()
    {
        _currentLifeTime = _lifeTime;

        _visability = GetComponent<CheckCameraVisability>();
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime < 0 || _checkVisability && !_visability.IsVisible)
            ReturnToPool();

        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vibration.Vibrate(1);

        DoDamage(collision.gameObject.GetComponent<IDamageable>());
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage(_damage);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        ParentPool.Pool.Enqueue(gameObject);
        gameObject.SetActive(false);
    }
}