using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LoadingScene"); // oyun sahnesine geç
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapatýldý.");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsMenu"); // ayarlar sahnesine geç
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("Controls"); // kontroller sahnesine geç
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits"); // yeni ekleyeceðin Credits sahnesine geç
    }
}
