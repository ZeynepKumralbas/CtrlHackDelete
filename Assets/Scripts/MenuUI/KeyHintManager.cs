using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class KeyHintManager : MonoBehaviour
{
    public static KeyHintManager Instance;

    public TextMeshProUGUI hintText;
    public CanvasGroup hintGroup;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;
    public bool showBlinkingCursor = true;
    public string cursorChar = "_";
    public float cursorBlinkInterval = 0.5f;

    private Coroutine typingRoutine;
    private bool cursorBlinking = false;

    void Awake()
    {
        Instance = this;
        hintGroup.alpha = 0f;
        hintText.text = "";
    }

    public void ShowHint(string message)
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
        }

        hintText.text = "";
        hintGroup.DOFade(1f, 0.2f).SetUpdate(true);

        typingRoutine = StartCoroutine(TypeTextRoutine(message));
    }

    public void HideHint()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
        }

        hintGroup.DOFade(0f, 0.2f).SetUpdate(true);
        hintText.text = "";
        cursorBlinking = false;
    }

    private IEnumerator TypeTextRoutine(string message)
    {
        cursorBlinking = false;
        string currentText = "";

        for (int i = 0; i < message.Length; i++)
        {
            currentText += message[i];
            hintText.text = currentText;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        if (showBlinkingCursor)
        {
            cursorBlinking = true;
            StartCoroutine(BlinkCursor(currentText));
        }
    }

    private IEnumerator BlinkCursor(string baseText)
    {
        bool visible = true;
        while (cursorBlinking)
        {
            hintText.text = baseText + (visible ? cursorChar : "");
            visible = !visible;
            yield return new WaitForSecondsRealtime(cursorBlinkInterval);
        }
    }
}
