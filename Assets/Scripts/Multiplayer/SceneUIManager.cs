using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUIManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        if (scene.name == "Menu")
        {
            // Menü sahnesine gelindiğinde video background ve loading açılır
            MenuManager.instance.OpenMenu("VideoBackground");
            MenuManager.instance.OpenMenu("LoadingMenu");
        }
        else if (scene.name == "MainMenu")
        {
            // Ana menüde sadece arkaplan açılabilir (eğer varsa)
            MenuManager.instance.OpenMenu("VideoBackground");
            MenuManager.instance.OpenMenu("TitleMenu"); // varsa aç
        }
        else if (scene.name == "Game")
        {
            // Oyun sahnesine geçince tüm menüler kapatılır, sadece oyun UI’si çalışır
            MenuManager.instance.CloseAllMenus();
        }
    }
}
