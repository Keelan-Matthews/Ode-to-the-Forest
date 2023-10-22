using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletHellProperties", menuName = "ScriptableObjects/BulletHellProperties", order = 1)]
public class BulletHellProperties : ScriptableObject
{
    public float fireForce;
    public int burstCount;
    public int projectilesPerBurst;
    [Range(0, 180)] public float angleSpread;
    public float startingDistance;
    public float timeBetweenBursts;
    public float restTime;
    public bool stagger;
    public bool oscillate;
    public bool targetPlayer;
}
