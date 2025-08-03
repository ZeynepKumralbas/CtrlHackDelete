using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskProgressText : MonoBehaviour
{
    public enum CursorType { Underscore, BlinkingLine }

    [Header("UI Referanslarý")]
    public Slider taskSlider;
    public TextMeshProUGUI taskText;

    [Header("Görev Metinleri")]
    [TextArea(2, 5)]
    public string[] taskMessages;

    [Header("Yazým Ayarlarý")]
    public float typingSpeed = 0.05f;
    public CursorType cursorType = CursorType.Underscore;

    private Coroutine typingCoroutine;
    private bool isBlinking = false;

    private void Update()
    {
        // Slider aktifse ve görünüyorsa yazý baþlasýn
        if (taskSlider.gameObject.activeInHierarchy && taskSlider.gameObject.activeSelf)
        {
            if (!taskText.gameObject.activeSelf)
            {
                taskText.gameObject.SetActive(true);
                StartNewTyping();
            }
        }
        else
        {
            if (taskText.gameObject.activeSelf)
            {
                taskText.gameObject.SetActive(false);
                StopTyping();
            }
        }
    }

    private void StartNewTyping()
    {
        StopTyping(); // Öncekini durdur
        if (taskMessages.Length == 0) return;
        string newMessage = taskMessages[Random.Range(0, taskMessages.Length)];
        typingCoroutine = StartCoroutine(TypeText(newMessage));
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isBlinking = false;
        taskText.text = "";
    }

    IEnumerator TypeText(string message)
    {
        taskText.text = "";
        isBlinking = false;

        for (int i = 0; i < message.Length; i++)
        {
            taskText.text = message.Substring(0, i + 1) + GetCursorSymbol();
            yield return new WaitForSeconds(typingSpeed);
        }

        isBlinking = true;
        StartCoroutine(BlinkCursor(message));
    }

    IEnumerator BlinkCursor(string message)
    {
        string cursor = GetCursorSymbol();
        while (isBlinking)
        {
            taskText.text = message + cursor;
            yield return new WaitForSeconds(0.5f);
            taskText.text = message + " ";
            yield return new WaitForSeconds(0.5f);
        }
    }

    private string GetCursorSymbol()
    {
        return cursorType == CursorType.Underscore ? "_" : "|";
    }
}
