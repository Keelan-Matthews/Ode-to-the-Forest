using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellController : MonoBehaviour
{
    [SerializeField] private float fireForce = 7f;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private int projectilesPerBurst = 3;
    [SerializeField] [Range(0, 359)] private float spreadAngle;
    [SerializeField] private float startingDistance = 0.1f;
    [SerializeField] private float timeBetweenBursts = 0.5f;
    [SerializeField] private float restTime = 1f;

    private bool _isShooting;

    private IEnumerator ShootBurst()
    {
        _isShooting = true;

        for (var i = 0; i < burstCount; i++)
        {
            // Get a bullet instance from the pool and set its position to the player's position
            var obj = ObjectPooler.Instance.GetPooledObject();
            if (obj == null) yield break;

            obj.GetComponent<BulletController>().SetAnimatorElectric();
            obj.GetComponent<BulletController>().isEnemyBullet = true;

            var t = transform;
            obj.transform.position = t.position;
            obj.transform.rotation = t.rotation;
            obj.SetActive(true);

            // Shoot the object towards the player
            var direction = (PlayerController.Instance.transform.position - transform.position).normalized;

            // Fire the bullet
            obj.GetComponent<Rigidbody2D>().velocity = direction * fireForce;

            yield return new WaitForSeconds(timeBetweenBursts);
        }

        yield return new WaitForSeconds(restTime);
        _isShooting = false;
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
}