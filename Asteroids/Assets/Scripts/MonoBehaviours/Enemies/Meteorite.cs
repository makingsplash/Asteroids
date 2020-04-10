using System.Collections.Generic;
using UnityEngine;

public class Meteorite : BaseEnemy
{
    [Header("When destroyed")]
    [SerializeField] private List<GameObject> _smallerMeteoritesPrefabs = new List<GameObject>();


    private void LateUpdate()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public override void TakeDamage()
    {
        if (_smallerMeteoritesPrefabs.Count > 0)
            SpawnSmallerMeteorites();

        base.TakeDamage();
        Destroy(gameObject);
    }

    void SpawnSmallerMeteorites()
    {
        for (int i = 0; i < UnityEngine.Random.Range(1, 3); i++)
        {
            // pick random prefab, apply random offset and rotation
            Instantiate(
                _smallerMeteoritesPrefabs[UnityEngine.Random.Range(0, _smallerMeteoritesPrefabs.Count)],
                transform.position + Vector3.right * UnityEngine.Random.Range(-0.2f, 0.2f) + Vector3.up * UnityEngine.Random.Range(-0.2f, 0.2f),
                Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-15, 15)));

            EnemySpawner.EnemiesSpawned++;
        }
    }
}