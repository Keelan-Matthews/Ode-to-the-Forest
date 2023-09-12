using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellProperties : ScriptableObject
{
    public float fireForce;
    public int burstCount;
    public int projectilesPerBurst;
    public float angleSpread;
    public float startingDistance;
    public float timeBetweenBursts;
    public float restTime;
    public bool stagger;
    public bool oscillate;
}
