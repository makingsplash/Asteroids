using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : BaseEnemy, IPoolObject
{
    [Header("When destroyed")]
    public List<MeteoriteType_SO> SmallerMeteoritesInfo = new List<MeteoriteType_SO>();

    public Coroutine ShiftingRoutine;

    [HideInInspector] public ObjectPool ParentPool { get ; set; }

    private float _camHeigth;
    private float _camWidth;
    public bool _isVisible; /////
    public OverBorderShifting _overBorder;  ////////
    private PolygonCollider2D _polygonCollider2d;
    private bool _isColliding;


    private void OnEnable()
    {
        if(_camHeigth == 0)
        {
            _camHeigth = CameraInfo.Instance.CamOrtSize;
            _camWidth = CameraInfo.Instance.CamAspect * _camHeigth;
        }

        _polygonCollider2d = GetComponent<PolygonCollider2D>();
        _polygonCollider2d.enabled = false;

        _overBorder = GetComponent<OverBorderShifting>();
        _overBorder.enabled = false;

        _isColliding = false;
    }

    private void OnDisable()
    {
        _overBorder.enabled = false;
        _isVisible = false;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);
    }

    // не нужно запускать в onEnable, сделать public и запускать снаружи
    public IEnumerator EnableShifting()
    {
        Debug.Log("+++ Started EnableShifting");

        WaitForSeconds wait = new WaitForSeconds(0.1f);

        _isVisible = false;
        while (!_isVisible && gameObject.activeSelf)
        {
            float posX = Mathf.Abs(transform.position.x);
            float posY = Mathf.Abs(transform.position.y);

            Debug.Log("X " + posX + " " + _camWidth);
            Debug.Log("Y " + posY + " " + _camHeigth);

            if(posX < _camWidth && posY < _camHeigth)
            {
                Debug.Log("ооо Became visible");
                _isVisible = true;
                _overBorder.enabled = true;
                _polygonCollider2d.enabled = true;
                break;
            }

            yield return wait;
        }

        Debug.Log("--- Stop coroutine");
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public override void TakeDamage()
    {
        if(!_isColliding)
        {
            _isColliding = true;

            if (SmallerMeteoritesInfo.Count > 0)
                SpawnSmallerMeteorites();

            base.TakeDamage();

            gameObject.SetActive(false);
            ParentPool.Pool.Enqueue(gameObject);
        }
    }

    void SpawnSmallerMeteorites()
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
            metComponent.Speed = metInfo.speed;
            metComponent.ScorePoints = metInfo.scorePoints;
            metComponent.SmallerMeteoritesInfo = metInfo.smallerMeteoritesSO;
            metComponent.ShiftingRoutine = StartCoroutine(metComponent.EnableShifting());

            EnemySpawner.EnemiesSpawned++;
        }
    }
}