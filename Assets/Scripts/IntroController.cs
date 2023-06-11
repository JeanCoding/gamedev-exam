using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    // Sets the variable for the time to be waited until it loads the next scene
    private float waitTime = 3f; 

    private void Start()
    {
        // This starts the waitForIntro function
        StartCoroutine(WaitForIntro());
    }

    private IEnumerator WaitForIntro()
    {
        yield return new WaitForSeconds(waitTime); // Wait for the specified time (waitTime) before starting the next line of code
        SceneManager.LoadScene("MainMenu"); // Load the "MainMenu" scene using Unity's SceneManager
    }
}
