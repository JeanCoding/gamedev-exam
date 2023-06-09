using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    // Load Scene
    public void Play() 
    {
        SceneManager.LoadScene("basketball-scene");
    }

    // Quit the game
    public void Quit()
    {
        Application.Quit();
    }

}
