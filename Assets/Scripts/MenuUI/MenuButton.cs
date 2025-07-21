using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    private Vector3 originalScale;
    private Image buttonImage;
    private Color originalColor;
    public Color hoverColor = Color.red; // Dilediðin rengi burada ayarla

    void Start()
    {
        originalScale = transform.localScale;

        // Eðer Image bileþeni varsa al
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
            originalColor = buttonImage.color;
    }

    public void OnHover()
    {
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);

        if (gameObject.name == "LeaveRoomButton" && buttonImage != null)
        {
            buttonImage.DOColor(hoverColor, 0.2f);
        }
    }

    public void OnExit()
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InBack);

        if (gameObject.name == "LeaveRoomButton" && buttonImage != null)
        {
            buttonImage.DOColor(originalColor, 0.2f);
        }
    }
}
