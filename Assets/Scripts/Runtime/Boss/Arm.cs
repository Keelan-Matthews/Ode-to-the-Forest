using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    [SerializeField] private BossController bossController;
    public bool isExposed;

    public void TakeDamage(int damage)
    {
        if (!isExposed) return;
        bossController.UpdateHealthBar(damage);
    }
}
