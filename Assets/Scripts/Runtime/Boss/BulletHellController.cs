using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellController : MonoBehaviour
{
    [SerializeField] private float fireForce;
    [SerializeField] private int burstCount;
    [SerializeField] private int projectilesPerBurst;
    [SerializeField] [Range(0, 359)] private float angleSpread;
    [SerializeField] private float startingDistance = 0.1f;
    [SerializeField] private float timeBetweenBursts;
    [SerializeField] private float restTime;

    private bool _isShooting;

    private IEnumerator ShootBurst()
    {
        _isShooting = true;
        
        float startAngle;
        float currentAngle;
        float angleStep;
        
        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep);

        for (var i = 0; i < burstCount; i++)
        {
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
            }
            
            currentAngle = startAngle;

            yield return new WaitForSeconds(timeBetweenBursts);
            
            TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep);
        }
        
        yield return new WaitForSeconds(restTime);
        _isShooting = false;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep)
    {
        var targetDirection = -transform.right;
        var targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        var endAngle = targetAngle;
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