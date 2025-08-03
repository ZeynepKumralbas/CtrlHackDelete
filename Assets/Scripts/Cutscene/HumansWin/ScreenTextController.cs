using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI screenText; // UI Text bileşeni

    private Coroutine typingCoroutine;
    private Coroutine blinkLoop;

    public void ClearText()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        screenText.text = "";
    }

    public void SetStaticText(string content)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        screenText.text = content;
    }

    public void StartTyping(string content, float charDelay = 0.05f)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeTextRoutine(content, charDelay));
    }

    private IEnumerator TypeTextRoutine(string content, float charDelay)
    {
        screenText.text = "";
        foreach (char c in content)
        {
            screenText.text += c;
            yield return new WaitForSeconds(charDelay);
        }
    }

    public void BlinkText(int blinkCount, float interval)
    {
        StartCoroutine(BlinkRoutine(blinkCount, interval));
    }

    private IEnumerator BlinkRoutine(int blinkCount, float interval)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            screenText.enabled = false;
            yield return new WaitForSeconds(interval);
            screenText.enabled = true;
            yield return new WaitForSeconds(interval);
        }
    }


public void StartBlinkLoop(float interval)
{
    if (blinkLoop != null) StopCoroutine(blinkLoop);
    blinkLoop = StartCoroutine(BlinkLoopCoroutine(interval));
}

public void StopBlinkLoop()
{
    if (blinkLoop != null)
    {
        StopCoroutine(blinkLoop);
        screenText.enabled = true; // sonunda görünür olsun
        blinkLoop = null;
    }
}

private IEnumerator BlinkLoopCoroutine(float interval)
{
    while (true)
    {
        screenText.enabled = !screenText.enabled;
        yield return new WaitForSeconds(interval);
    }
}

}
