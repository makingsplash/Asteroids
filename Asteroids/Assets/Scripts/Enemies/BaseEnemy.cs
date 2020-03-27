using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    private static int _enemyAmount;
    // Тоже private ?
    public static int EnemyAmount
    {
        get
        {
            return _enemyAmount;
        }

        protected set
        {
            _enemyAmount = value;
        }
    }
}
