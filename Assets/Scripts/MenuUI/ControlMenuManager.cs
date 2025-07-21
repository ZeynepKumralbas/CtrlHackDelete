using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlMenuManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
