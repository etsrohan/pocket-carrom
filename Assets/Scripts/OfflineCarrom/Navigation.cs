using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void restartGameOffline()
    {
        SceneManager.LoadScene("OfflineCarrom");
    }

    public void restartGameSingle()
    {
        SceneManager.LoadScene("SingleCarrom");
    }

}
