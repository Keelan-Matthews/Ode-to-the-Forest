using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void BackToHome()
    {
        ScenesManager.LoadScene("Home");
    }
    
    public void SpeedUp()
    {
        animator.speed = 2;
    }
    
    public void SlowDown()
    {
        animator.speed = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToHome();
        }
        
        // Speed up while they are holding down the space bar
        if (Input.GetKey(KeyCode.E))
        {
            SpeedUp();
        }
        else
        {
            SlowDown();
        }
    }
}
