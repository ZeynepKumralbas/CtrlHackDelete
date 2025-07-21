using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Menu"); // oyun sahnenin ad�n� yaz
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapat�ld�.");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsMenu"); // ayarlar sahnesine ge�
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("Controls"); // kontroller sahnesine ge�
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits"); // yeni ekleyece�in Credits sahnesine ge�
    }
}
