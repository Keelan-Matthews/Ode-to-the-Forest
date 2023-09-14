using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int coresDestroyed;
    public bool isDead;

    private void Awake()
    {
        CoreController.OnCoreDestroyed += CheckCores;
    }

    private void CheckCores()
    {
        coresDestroyed++;
    }
}
