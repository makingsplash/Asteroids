using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : BaseEnemy
{

    [SerializeField] private float _speed;

    private new Rigidbody2D rigidbody;

    [SerializeField] private List<GameObject> _mediumMeteoritPrefabs;
    [SerializeField] private GameObject _littleMeteoritePrefab;
    private enum MeteoriteTypes
    {
        BigMeteorite,
        MediumMeteorite,
        LittleMeteorite
    }
    private MeteoriteTypes _meteoriteType = MeteoriteTypes.BigMeteorite;


    private void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        rigidbody.velocity = transform.up * _speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public override void TakeDamage()
    {
        switch (_meteoriteType)
        {
            case MeteoriteTypes.BigMeteorite:
                // Нужно заменить события на это и удалить везде подписки
                UIManager.Instance.ChangeScore(10);
                // Спавним 1-2 средних метеорита
                for (int i = 0; i < UnityEngine.Random.Range(1, 3); i++)
                {
                    if (_mediumMeteoritPrefabs != null)
                    {
                        GameObject mediumMeteorite = Instantiate(
                            // Рандомим какой из префабов для среднего метеорита будем использовать
                            _mediumMeteoritPrefabs[UnityEngine.Random.Range(0, _mediumMeteoritPrefabs.Count)],
                            transform.position,
                            // Рандомно немного меняем угол полёта
                            Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-30, 30)));
                        Meteorite meteoriteMediumController = mediumMeteorite.GetComponent<Meteorite>();

                        // Рандомно немного увеличиваем скорость передвижения
                        meteoriteMediumController._speed = _speed + (UnityEngine.Random.Range(20, 60));
                        meteoriteMediumController._meteoriteType = MeteoriteTypes.MediumMeteorite;
                    }
                    else
                        Debug.LogError("Не заполнен List с префабами средних метеоритов");
                }
                break;

            case MeteoriteTypes.MediumMeteorite:
                UIManager.Instance.ChangeScore(15);
                // Спавним 1 маленький метеорит
                GameObject littleMeteorite = Instantiate(_littleMeteoritePrefab,
                            transform.position,
                            // Рандомно немного меняем угол полёта
                            Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-30, 30)));
                Meteorite meteoriteLittleController = littleMeteorite.GetComponent<Meteorite>();

                // Рандомно немного увеличиваем скорость передвижения
                meteoriteLittleController._speed = _speed + (UnityEngine.Random.Range(20, 60));
                meteoriteLittleController._meteoriteType = MeteoriteTypes.LittleMeteorite;
                break;

            case MeteoriteTypes.LittleMeteorite:
                UIManager.Instance.ChangeScore(25);
                break;
        }

        base.TakeDamage();
        Destroy(gameObject);
    }
}