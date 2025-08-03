using UnityEngine;
using UnityEngine.UI;

public class AutoScrollCredits : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 20f; // px/sn
    public float restartDelay = 2f; // sona geldiðinde bekleme süresi

    private RectTransform contentRect;
    private float contentHeight;
    private float viewportHeight;
    private float restartTimer;

    private void Start()
    {
        contentRect = scrollRect.content;
        contentHeight = contentRect.rect.height;
        viewportHeight = scrollRect.viewport.rect.height;

        // Baþlangýçta en altta ol
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void Update()
    {
        // Scroll pozisyonunu manuel ayarlýyoruz
        float newY = scrollRect.verticalNormalizedPosition - (scrollSpeed / contentHeight) * Time.deltaTime;
        scrollRect.verticalNormalizedPosition = newY;

        // Eðer sona geldiyse
        if (scrollRect.verticalNormalizedPosition <= 0f)
        {
            restartTimer += Time.deltaTime;
            if (restartTimer >= restartDelay)
            {
                scrollRect.verticalNormalizedPosition = 1f; // Baþa dön
                restartTimer = 0f;
            }
        }
    }
}
