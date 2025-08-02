using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FinalSequenceController : MonoBehaviour
{
    [SerializeField] private ScreenTextController mainScreen;
    [SerializeField] private ScreenTextController[] otherScreens;

    [SerializeField] private float delayAfterInteract = 1.2f;
    public CutsceneCameraController cutsceneCameraController;
    [SerializeField] private Image[] screenImages;
    [SerializeField] private Material shutdownMaterial;

    void Start()
    {
        // Ana ekran başta boş
        mainScreen.ClearText();

        // Diğer ekranlara özel metinleri atayalım
        if (otherScreens.Length >= 3)
        {
            otherScreens[2].SetStaticText("Eymen Arapoğlu - Developer\nŞevval Arslan - Developer\nZeynep Kumralbaş - Developer & Scrum Master\nEren Altınışık - UI/UX Designer\nFurkan Kurnaz - Level & Game Designer"); // Developer ekibi
            otherScreens[0].SetStaticText("GLOBAL STATUS: CONNECTED\nHUMAN ACTIVITY INDEX: 98%");
            otherScreens[1].SetStaticText("WARNING: HUMAN BREACH DETECTED\nSEARCHING FOR IMPOSTORS...");
            otherScreens[3].SetStaticText("WARNING: HUMAN BREACH DETECTED\nSEARCHING FOR IMPOSTORS...");
        }

        foreach (var screen in otherScreens)
        {
            screen.StartBlinkLoop(0.4f); // açılışta 4 kez yanıp sön
        }
    }

    public void StartHackSequence()
    {
        StartCoroutine(HackSequenceRoutine());
    }

    private IEnumerator HackSequenceRoutine()
    {
        yield return new WaitForSeconds(delayAfterInteract);

        // Artık kamera coroutine’ini çağırmıyoruz, çünkü olaylara göre tetiklenecek
        // StartCoroutine(cutsceneCameraController.PlayCutsceneCameraSequence());

        mainScreen.BlinkText(3, 0.15f);
        foreach (var screen in otherScreens)
        {
            screen.StopBlinkLoop();
            screen.BlinkText(8, 0.25f); // Kapanış blink'i
        }
        
        // yazılar blink etti, sonra:
        SetScreenMaterialsToShutdown();

        yield return new WaitForSeconds(2f);

        string[] shutdownLines = new string[]
        {
            "CTRL HACK DELETE",
            "SHUTTING DOWN SYSTEM...",
            "MEMORY BLOCKS ERASED...",
            "ROBOT DIRECTIVES PURGED...",
            "TERMINATING CONTROL CORE..."
        };

        for (int i = 0; i < shutdownLines.Length; i++)
        {
            foreach (var screen in otherScreens)
            {
                screen.StartTyping(shutdownLines[i]);
            }

            yield return new WaitForSeconds(1.5f);
        }

        yield return new WaitForSeconds(1f);

        foreach (var screen in otherScreens)
        {
            screen.StartTyping("SYSTEM OVERRIDE COMPLETE.\nTHANK YOU, HUMAN.");
        }

        yield return new WaitForSeconds(1f);
    }

    public void OnLeadCharacterPressedButton()
    {
        // Burası artık gerekliyse kullanılabilir, ama kamerayı burada başlatmıyoruz
        // StartCoroutine(cutsceneCameraController.PlayCutsceneCameraSequence());
    }

    private void SetScreenMaterialsToShutdown()
    {
        foreach (var image in screenImages)
        {
            image.material = shutdownMaterial;
        }
    }

}
