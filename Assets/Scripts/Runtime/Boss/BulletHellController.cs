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

    private bool _isShooting;

    private IEnumerator ShootBurst()
    {
        _isShooting = true;

        var timeBetweenProjectiles = 0f;
        
        TargetConeOfInfluence(out var startAngle, out var currentAngle, out var angleStep, out var endAngle);

        if (stagger)
        {
            timeBetweenProjectiles = timeBetweenBursts / projectilesPerBurst;
        }

        for (var i = 0; i < burstCount; i++)
        {
            if (!oscillate)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }

            switch (oscillate)
            {
                case true when i % 2 != 1:
                    TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
                    break;
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
        _isShooting = false;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        var targetDirection = -transform.right;
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

    private void Shoot()
    {
        if (_isShooting) return;
        StartCoroutine(ShootBurst());
    }

    private void Update()
    {
        Shoot();
    }

    private Vector2 FindBulletSpawnPos(float currentAngle)
    {
        var x = transform.position.x + startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        var y = transform.position.y + startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        var pos = new Vector2(x, y);

        return pos;
    }
}