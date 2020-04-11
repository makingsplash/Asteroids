using UnityEngine;

public class OverBorderShifting : MonoBehaviour
{
    // Коррекция позиции при выходе за границы камеры 
    // Без неё мы будем постоянно перемещать вышедший объект на границу камеры
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

        if (_posY > _camOrtSize)                        // Объект вышел сверху
            transform.position = new Vector2(_posX, -_posY + _shiftCorrection);
        if (_posY < -_camOrtSize)                       // Объект вышел снизу
            transform.position = new Vector2(_posX, -_posY - _shiftCorrection);
        if (_posX > _camOrtSize * Camera.main.aspect)   // Объект вышел справа
            transform.position = new Vector2(-_posX + _shiftCorrection, _posY);
        if (_posX < -_camOrtSize * Camera.main.aspect)  // Объект вышел слева
            transform.position = new Vector2(-_posX - _shiftCorrection, _posY);
    }
}
