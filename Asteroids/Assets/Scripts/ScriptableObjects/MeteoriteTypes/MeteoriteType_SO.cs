using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeteoriteType", menuName = "MeteoriteType", order = 51)]
public class MeteoriteType_SO : ScriptableObject
{
    public Sprite sprite;
    public PolygonCollider2D polygonCollider2d;
    
    public float speed;
    public byte scorePoints;

    public List<MeteoriteType_SO> smallerMeteoritesSO = new List<MeteoriteType_SO>();
}
