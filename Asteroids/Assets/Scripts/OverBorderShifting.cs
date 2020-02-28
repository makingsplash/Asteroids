using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverBorderShifting : MonoBehaviour
{
    // Коррекция позиции при выходе лазера за границы камеры 
    // Без неё мы будем постоянно перемещать вышедший за границу камеры лазер на границу камеры
    private const float _shiftCorrection = 0.2f;

    private float _camOrtSize;  // Camera.main.orthographicSize
    private float _posX;        // transform.position.x
    private float _posY;        // transform.position.y


    private void Start()
    {
        _camOrtSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        _posX = transform.position.x;
        _posY = transform.position.y;

        if (_posY > _camOrtSize)                        // Лазер вышел сверху
            transform.position = new Vector2(_posX, -_posY + _shiftCorrection);
        if (_posY < -_camOrtSize)                       // Лазер вышел снизу
            transform.position = new Vector2(_posX, -_posY - _shiftCorrection);
        if (_posX > _camOrtSize * Camera.main.aspect)   // Лазер вышел справа
            transform.position = new Vector2(-_posX + _shiftCorrection, _posY);
        if (_posX < -_camOrtSize * Camera.main.aspect)  // Лазер вышел слева
            transform.position = new Vector2(-_posX - _shiftCorrection, _posY);
    }
}
