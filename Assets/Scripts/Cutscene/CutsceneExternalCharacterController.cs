using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneExternalCharacterController : MonoBehaviour
{
    [SerializeField] private CutsceneHumanExternalMovement[] characters;
    public float runningTime = 2f;
    public float sneakingTime = 2f;

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // İleriye doğru koş
        Vector2 forward = new Vector2(0, 1);
        foreach (var c in characters)
            c.SetExternalInput(forward, isRunning: true, isSneaking: false);

        yield return new WaitForSeconds(runningTime);

        // Sağa doğru sneaking
        Vector2 right = new Vector2(1, 0);
        foreach (var c in characters)
            c.SetExternalInput(right, isRunning: false, isSneaking: true);

        yield return new WaitForSeconds(sneakingTime);

    }
}
