using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LoadingScene"); // oyun sahnenin ad�n� yaz
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapat�ld�.");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsMenu"); // Ayarlar sahnesine ge�
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("Controls"); // Kontroller sahnesine ge�
    }
}
