using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    private Vector3 originalScale;
    private Image buttonImage;
    private Color originalColor;
    public Color hoverColor = Color.red;

    // Hangi butonlarda renk deðiþecekse buraya ekle
    private readonly string[] colorChangeButtons = { "LeaveRoomButton", "LeaveGameButton", "QuitGameButton" };

    void Start()
    {
        originalScale = transform.localScale;

        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
            originalColor = buttonImage.color;
    }

    public void OnHover()
    {
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);

        if (ShouldChangeColor() && buttonImage != null)
        {
            buttonImage.DOColor(hoverColor, 0.2f);
        }
    }

    public void OnExit()
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InBack);

        if (ShouldChangeColor() && buttonImage != null)
        {
            buttonImage.DOColor(originalColor, 0.2f);
        }
    }

    private bool ShouldChangeColor()
    {
        foreach (var name in colorChangeButtons)
        {
            if (gameObject.name == name)
                return true;
        }
        return false;
    }
}
