using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    void Start()
    {
        // Change to the Main Menu after 4 seconds
        StartCoroutine(ChangeToMainMenu());
    }
    
    private IEnumerator ChangeToMainMenu()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("MainMenu");
    }
}
