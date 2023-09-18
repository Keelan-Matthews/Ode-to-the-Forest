using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellController : MonoBehaviour
{
    [SerializeField] private float fireForce;
    [SerializeField] private int burstCount;
    [SerializeField] private int projectilesPerBurst;
    [SerializeField] [Range(0, 170)] private float angleSpread;
    [SerializeField] private float startingDistance = 0.1f;
    [SerializeField] private float timeBetweenBursts;
    [SerializeField] private float restTime;
    [SerializeField] private bool stagger;
    [SerializeField] private bool oscillate;
    [SerializeField] private bool targetPlayer;
    
    [Header("Bullet Attack 1")]
    [SerializeField] private List<BulletHellProperties> bulletHellProperties1;
    [Header("Bullet Attack 2")]
    [SerializeField] private List<BulletHellProperties> bulletHellProperties2;
    [Header("Bullet Attack 3")]
    [SerializeField] private List<BulletHellProperties> bulletHellProperties3;
    [Header("Bullet Attack 4")]
    [SerializeField] private List<BulletHellProperties> bulletHellProperties4;

    [SerializeField] private bool isEnraged;

    private int _currentBulletHellPropertiesIndex;
    public bool cycleEnded;

    public bool isShooting;

    private IEnumerator ShootBurst(int attackNumber)
    {
        isShooting = true;

        var timeBetweenProjectiles = 0f;
        
        TargetConeOfInfluence(out var startAngle, out var currentAngle, out var angleStep, out var endAngle, attackNumber);

        if (stagger)
        {
            timeBetweenProjectiles = timeBetweenBursts / projectilesPerBurst;
        }
        
        if (cycleEnded) yield break;

        for (var i = 0; i < burstCount; i++)
        {
            if (!oscillate)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle, attackNumber, false);
            }

            switch (oscillate)
            {
                // case true when i % 2 != 1:
                //     TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
                //     break;
                case true:
                    currentAngle = endAngle;
                    endAngle = startAngle;
                    startAngle = currentAngle;
                    angleStep *= -1;
                    break;
            }

            for (var j = 0; j < projectilesPerBurst; j++)
            {
                var pos = FindBulletSpawnPos(currentAngle);
            
                // Get a bullet instance from the pool and set its position to the player's position
                var obj = ObjectPooler.Instance.GetPooledObject();
                if (obj == null) yield break;

                obj.GetComponent<BulletController>().SetAnimatorElectric();
                obj.GetComponent<BulletController>().isEnemyBullet = true;

                obj.transform.position = pos;
                obj.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                obj.SetActive(true);

                // Shoot the object exactly to the left
                var direction = (obj.transform.position - transform.position).normalized;

                // Fire the bullet
                obj.GetComponent<Rigidbody2D>().velocity = direction * fireForce;

                currentAngle += angleStep;
                
                if (stagger)
                {
                    yield return new WaitForSeconds(timeBetweenProjectiles);
                }
            }
            
            currentAngle = startAngle;

            if (!stagger)
            {
                yield return new WaitForSeconds(timeBetweenBursts);
            }
        }
        
        yield return new WaitForSeconds(restTime);
        isShooting = false;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle, int attackNumber = 1, bool cycle = true)
    {
        if (cycle)
        {
            CycleBulletHellProperties(attackNumber);
        }
        
        var targetDirection = targetPlayer ? (PlayerController.Instance.transform.position - transform.position).normalized : -transform.right;
        var targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        endAngle = targetAngle;
        currentAngle = targetAngle;
        var halfAngleSpread = 0f;
        angleStep = 0f;

        if (angleSpread != 0)
        {
            angleStep = angleSpread / (projectilesPerBurst - 1);
            halfAngleSpread = angleSpread / 2f;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    private void CycleBulletHellProperties(int attackNumber)
    {
        switch (attackNumber)
        {
            case 1:
                if (bulletHellProperties1.Count == 0) return;
                CycleBulletHellProperties(bulletHellProperties1);
                break;
            case 2:
                if (bulletHellProperties2.Count == 0) return;
                CycleBulletHellProperties(bulletHellProperties2);
                break;
            case 3:
                if (bulletHellProperties3.Count == 0) return;
                CycleBulletHellProperties(bulletHellProperties3);
                break;
            case 4:
                if (bulletHellProperties4.Count == 0) return;
                CycleBulletHellProperties(bulletHellProperties4);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackNumber), attackNumber, null);
        }
    }
    
    private void CycleBulletHellProperties(List<BulletHellProperties> bulletHellProperties)
    {
        if (_currentBulletHellPropertiesIndex >= bulletHellProperties.Count)
        {
            _currentBulletHellPropertiesIndex = 0;
            cycleEnded = true;
            return;
        }
        
        fireForce = bulletHellProperties[_currentBulletHellPropertiesIndex].fireForce;
        burstCount = bulletHellProperties[_currentBulletHellPropertiesIndex].burstCount;
        projectilesPerBurst = bulletHellProperties[_currentBulletHellPropertiesIndex].projectilesPerBurst;
        angleSpread = bulletHellProperties[_currentBulletHellPropertiesIndex].angleSpread;
        startingDistance = bulletHellProperties[_currentBulletHellPropertiesIndex].startingDistance;
        timeBetweenBursts = bulletHellProperties[_currentBulletHellPropertiesIndex].timeBetweenBursts;
        restTime = bulletHellProperties[_currentBulletHellPropertiesIndex].restTime;
        stagger = bulletHellProperties[_currentBulletHellPropertiesIndex].stagger;
        oscillate = bulletHellProperties[_currentBulletHellPropertiesIndex].oscillate;
        targetPlayer = bulletHellProperties[_currentBulletHellPropertiesIndex].targetPlayer;
        
        _currentBulletHellPropertiesIndex++;
    }

    public void Shoot(int attackNumber)
    {
        if (isShooting) return;
        
        // If isEnraged and attackNumber is 1, 3 should be used
        // If isEnraged and attackNumber is 2, 4 should be used
        if (isEnraged)
        {
            attackNumber += 2;
        }
        StartCoroutine(ShootBurst(attackNumber));
    }

    private Vector2 FindBulletSpawnPos(float currentAngle)
    {
        var x = transform.position.x + startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        var y = transform.position.y + startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        var pos = new Vector2(x, y);

        return pos;
    }
}