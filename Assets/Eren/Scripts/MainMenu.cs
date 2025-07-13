using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LoadingScene"); // oyun sahnenin adýný yaz
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatýldý.");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsMenu"); // Ayarlar sahnesine geç
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("Controls"); // Kontroller sahnesine geç
    }
}
