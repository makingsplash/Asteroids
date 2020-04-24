using UnityEngine;

public class Laser : MonoBehaviour, IDamager, IPoolObject
{
    public ObjectPool ParentPool { get; set; }

    [SerializeField] private byte _damage;
    [SerializeField] private ushort _speed;
    [SerializeField] private float _lifeTime;
    private float _currentLifeTime;


    private void OnEnable()
    {
        _currentLifeTime = _lifeTime;
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

    void ReturnToPool()
    {
        gameObject.SetActive(false);
        ParentPool.Pool.Enqueue(gameObject);
    }
}