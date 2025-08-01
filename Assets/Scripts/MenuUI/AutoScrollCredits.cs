using UnityEngine;
using UnityEngine.UI;

public class AutoScrollCredits : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 20f; // px/sn
    public float restartDelay = 2f; // sona geldi�inde bekleme s�resi

    private RectTransform contentRect;
    private float contentHeight;
    private float viewportHeight;
    private float restartTimer;

    private void Start()
    {
        contentRect = scrollRect.content;
        contentHeight = contentRect.rect.height;
        viewportHeight = scrollRect.viewport.rect.height;

        // Ba�lang��ta en altta ol
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void Update()
    {
        // Scroll pozisyonunu manuel ayarl�yoruz
        float newY = scrollRect.verticalNormalizedPosition - (scrollSpeed / contentHeight) * Time.deltaTime;
        scrollRect.verticalNormalizedPosition = newY;

        // E�er sona geldiyse
        if (scrollRect.verticalNormalizedPosition <= 0f)
        {
            restartTimer += Time.deltaTime;
            if (restartTimer >= restartDelay)
            {
                scrollRect.verticalNormalizedPosition = 1f; // Ba�a d�n
                restartTimer = 0f;
            }
        }
    }
}
