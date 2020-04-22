using UnityEngine;

public class Laser : MonoBehaviour, IDamager, IPoolObject
{
    public ObjectPool ParentPool { get; set; }

    [SerializeField] private ushort _speed;
    [SerializeField] private float _lifeTime;

    [SerializeField]
    [Tooltip("If rocket laser, will add points to score for a hit")]
    private bool _isRocketLaser;

    private float _currentLifeTime;


    private void OnEnable()
    {
        _currentLifeTime = _lifeTime;
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime < 0)
        {
            ReturnToPool();
        }

        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (_isRocketLaser)
        {
            BaseEnemy enemy = collision.gameObject.GetComponent<BaseEnemy>();
            UIManager.Instance.ChangeScore(enemy.ScorePoints);
        }

        DoDamage(damageable);
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage();
        ReturnToPool();
    }
    void ReturnToPool()
    {
        gameObject.SetActive(false);
        ParentPool.Pool.Enqueue(gameObject);
    }
}