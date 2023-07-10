using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        PlayerController.Instance.SetInvincible(true);
    }
}
