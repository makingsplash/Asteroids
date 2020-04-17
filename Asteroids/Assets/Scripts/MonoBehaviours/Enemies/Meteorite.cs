using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : BaseEnemy, IPoolObject
{
    [Header("When destroyed")]
    public List<MeteoriteType_SO> SmallerMeteoritesInfo = new List<MeteoriteType_SO>();

    [HideInInspector] public OverBorderShifting OverBorder;
    [HideInInspector] public ObjectPool ParentPool { get ; set; }

    private float _camHeigth;
    private float _camWidth;
    private bool _isVisible;
    private Coroutine _shifting;


    private void Start()
    {
        _camHeigth = CameraInfo.Instance.CamOrtSize;
        _camWidth = CameraInfo.Instance.CamAspect * _camHeigth;
    }

    private void OnEnable()
    {
        if (OverBorder == null)
            OverBorder = GetComponent<OverBorderShifting>();
        
        OverBorder.enabled = false;
        _shifting = StartCoroutine(EnableShifting());
    }

    private void OnDisable()
    {
        StopCoroutine(_shifting);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);
    }

    private IEnumerator EnableShifting()
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);

        _isVisible = false;
        while (!_isVisible)
        {
            float posX = Mathf.Abs(transform.position.x);
            float posY = Mathf.Abs(transform.position.y);
            if(posX < _camWidth && posY < _camHeigth)
            {
                _isVisible = true;
                OverBorder.enabled = true;
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

    public override void TakeDamage()
    {
        if (SmallerMeteoritesInfo.Count > 0)
            SpawnSmallerMeteorites();

        base.TakeDamage();

        ParentPool.Pool.Enqueue(gameObject);
        gameObject.SetActive(false);
    }

    void SpawnSmallerMeteorites()
    {
        for (int i = 0; i < UnityEngine.Random.Range(1, 3); i++)
        {
            // pick random prefab, apply random offset and rotation
            GameObject newGO = ParentPool.SpawnObject(
                 transform.position + Vector3.right * UnityEngine.Random.Range(-0.2f, 0.2f) + Vector3.up * UnityEngine.Random.Range(-0.2f, 0.2f),
                 transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-15, 15));

            MeteoriteType_SO metInfo = SmallerMeteoritesInfo[UnityEngine.Random.Range(0, SmallerMeteoritesInfo.Count)];

            newGO.transform.localScale = metInfo.transform.localScale;
            newGO.GetComponent<SpriteRenderer>().sprite = metInfo.sprite;
            newGO.GetComponent<PolygonCollider2D>().points = metInfo.polygonCollider2d.points;

            Meteorite GOmetScr = newGO.GetComponent<Meteorite>();
            GOmetScr.Speed = metInfo.speed;
            GOmetScr.ScorePoints = metInfo.scorePoints;
            GOmetScr.SmallerMeteoritesInfo = metInfo.smallerMeteoritesSO;

            EnemySpawner.EnemiesSpawned++;
        }
    }
}