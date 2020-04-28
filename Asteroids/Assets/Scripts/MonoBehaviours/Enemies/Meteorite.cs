using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : BaseEnemy, IPoolObject, IDamageable
{
    [HideInInspector] public List<MeteoriteType_SO> SmallerMeteoritesInfo = new List<MeteoriteType_SO>();
    [HideInInspector] public ObjectPool ParentPool { get ; set; }
    [HideInInspector] public Coroutine ShiftRoutine;
    [HideInInspector] public GameObject Warning;

    private float _camHeigth;
    private float _camWidth;
    private OverBorderShift _overBorderShift;
    private PolygonCollider2D _polygonCollider2d;
    private bool _isColliding;


    private void OnEnable()
    {
        _camHeigth = CameraInfo.Instance.CamOrtSize;
        _camWidth = CameraInfo.Instance.CamAspect * _camHeigth;

        _isColliding = false;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);
    }

    public IEnumerator EnableShift()
    {
        if (_polygonCollider2d == null)
            _polygonCollider2d = GetComponent<PolygonCollider2D>();
        _polygonCollider2d.enabled = false;

        if (_overBorderShift == null)
            _overBorderShift = GetComponent<OverBorderShift>();
        _overBorderShift.enabled = false;

        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            float posX = Mathf.Abs(transform.position.x);
            float posY = Mathf.Abs(transform.position.y);

            if(posX < _camWidth && posY < _camHeigth)
            {
                _overBorderShift.enabled = true;
                _polygonCollider2d.enabled = true;

                Destroy(Warning);

                break;
            }
            yield return wait;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public void TakeDamage(byte damage)
    {
        if(!_isColliding)
        {
            _isColliding = true;

            UIManager.Instance.ChangeScore(HitScorePoints);
            DecreaseHealth(damage);
        }
        _isColliding = false;
    }

    protected override void Death()
    {
        UIManager.Instance.ChangeScore(DeathScorePoints);

        if (SmallerMeteoritesInfo.Count > 0)
            SpawnSmallerMeteorites();

        gameObject.SetActive(false);
        ParentPool.Pool.Enqueue(gameObject);
    }

    private void SpawnSmallerMeteorites()
    {
        for (int i = 0; i < UnityEngine.Random.Range(1, 3); i++)
        {
            // pick random prefab, apply random offset and rotation
            GameObject meteorite = ParentPool.SpawnObject(
                 transform.position + Vector3.right * UnityEngine.Random.Range(-0.2f, 0.2f) + Vector3.up * UnityEngine.Random.Range(-0.2f, 0.2f),
                 transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-15, 15));

            MeteoriteType_SO metInfo = SmallerMeteoritesInfo[UnityEngine.Random.Range(0, SmallerMeteoritesInfo.Count)];

            meteorite.transform.localScale = metInfo.transform.localScale;
            meteorite.GetComponent<SpriteRenderer>().sprite = metInfo.sprite;
            meteorite.GetComponent<PolygonCollider2D>().points = metInfo.polygonCollider2d.points;

            Meteorite metComponent = meteorite.GetComponent<Meteorite>();
            metComponent.Health = metInfo.health;
            metComponent.Speed = metInfo.speed;
            metComponent.HitScorePoints = metInfo.hitScorePoints;
            metComponent.DeathScorePoints = metInfo.deathScorePoints;
            metComponent.SmallerMeteoritesInfo = metInfo.smallerMeteoritesSO;
            metComponent.ShiftRoutine = StartCoroutine(metComponent.EnableShift());

            WaveManager.EnemiesSpawned++;
        }
    }
}