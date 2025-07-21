using TMPro;
using UnityEngine;
using System.Collections;

public class TerminalTyping : MonoBehaviour
{
    public TextMeshProUGUI terminalText;
    public float typingSpeed = 0.05f;

    private string fullCommand = "C:/User> initiate_hack.exe";
    private bool isBlinking = false;

    void Start()
    {
        StartCoroutine(TypeCommand());
    }

    IEnumerator TypeCommand()
    {
        terminalText.text = "";
        for (int i = 0; i < fullCommand.Length; i++)
        {
            terminalText.text += fullCommand[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        StartCoroutine(BlinkingCursor());
    }

    IEnumerator BlinkingCursor()
    {
        isBlinking = true;
        while (isBlinking)
        {
            terminalText.text = fullCommand + "_";
            yield return new WaitForSeconds(0.5f);
            terminalText.text = fullCommand + " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
