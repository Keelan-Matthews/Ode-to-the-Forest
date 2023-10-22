using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData.asset", menuName = "EnemyData", order = 0)]
public class EnemyData : ScriptableObject
{
    public float speed;
    public int damage;
    public int essenceToDrop;
}
