using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteController : MonoBehaviour, IDamagable
{
    public static Action OnMeteoriteTouchedPlayer;    // Метеорит коснулся игрока
    public static Action<int> OnMeteoriteWasShooted;  // Начисление очков за подстреленный метеорит

    [SerializeField] private float _speed;

    private Rigidbody2D rigidbody;

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
        if (collision.gameObject.tag == "Player")
            OnMeteoriteTouchedPlayer();
    }

    public void GotDamage()
    {
        switch (_meteoriteType)
        {
            case MeteoriteTypes.BigMeteorite:
                OnMeteoriteWasShooted(10);
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
                        MeteoriteController meteoriteMediumController = mediumMeteorite.GetComponent<MeteoriteController>();

                        // Рандомно немного увеличиваем скорость передвижения
                        meteoriteMediumController._speed = _speed + (UnityEngine.Random.Range(20, 60));
                        meteoriteMediumController._meteoriteType = MeteoriteTypes.MediumMeteorite;
                    }
                    else
                        Debug.LogError("Не заполнен List с префабами средних метеоритов");
                }
                break;

            case MeteoriteTypes.MediumMeteorite:
                OnMeteoriteWasShooted(15);
                // Спавним 1 маленький метеорит
                GameObject littleMeteorite = Instantiate(_littleMeteoritePrefab,
                            transform.position,
                            // Рандомно немного меняем угол полёта
                            Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-30, 30)));
                MeteoriteController meteoriteLittleController = littleMeteorite.GetComponent<MeteoriteController>();

                // Рандомно немного увеличиваем скорость передвижения
                meteoriteLittleController._speed = _speed + (UnityEngine.Random.Range(20, 60));
                meteoriteLittleController._meteoriteType = MeteoriteTypes.LittleMeteorite;
                break;

            case MeteoriteTypes.LittleMeteorite:
                OnMeteoriteWasShooted(25);
                break;
        }
        Destroy(gameObject);
    }
}