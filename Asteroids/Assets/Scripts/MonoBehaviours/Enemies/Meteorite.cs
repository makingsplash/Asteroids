using System.Collections.Generic;
using UnityEngine;

public class Meteorite : BaseEnemy, IPoolObject
{
    [Header("When destroyed")]
    public List<MeteoriteType_SO> SmallerMeteoritesInfo = new List<MeteoriteType_SO>();

    [HideInInspector] public ObjectPool ParentPool { get ; set; }

    private void Update()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);
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