using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _rocket;
    [SerializeField] private float _spawnTime;
    [SerializeField] private RectTransform[] _spawnPositionsFromUI = new RectTransform[3];

    private int _positionIndex = 0;

    public IEnumerator SpawnRocket()
    {
        yield return new WaitForSeconds(_spawnTime);
        UIManager.Instance.DisableMessage();


        RectTransform rectTransform = _spawnPositionsFromUI[_positionIndex];

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 spawnWorldPos = Camera.main.ScreenToWorldPoint(corners[0]) +
            (Camera.main.ScreenToWorldPoint(corners[2]) - Camera.main.ScreenToWorldPoint(corners[0])) / 2;

        _rocket.transform.position = spawnWorldPos;
        _rocket.transform.rotation = Quaternion.identity;

        UIManager.Instance.DecreaseLifes();
        _rocket.SetActive(true);
        

        _positionIndex++;
    }
}
