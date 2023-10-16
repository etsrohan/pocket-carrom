using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PracticeGame()
    {
        SceneManager.LoadScene("OfflineCarrom");
    }

    public void ClassicGame()
    {
        SceneManager.LoadScene("ClassicCarrom");
    }

    public void SingleGame()
    {
        SceneManager.LoadScene("SingleCarrom");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
