using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeteoriteType", menuName = "MeteoriteType", order = 51)]
public class MeteoriteType_SO : ScriptableObject
{
    public Transform transform;
    public Sprite sprite;
    public PolygonCollider2D polygonCollider2d;

    public byte health;
    public float speed;
    public byte scorePoints;

    public List<MeteoriteType_SO> smallerMeteoritesSO = new List<MeteoriteType_SO>();
}
