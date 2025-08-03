using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(KeyHintData))]
public class KeyHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private KeyHintData keyData;
    private Vector3 originalScale;
    private Image image;
    private Color originalColor;

    [Header("Animasyon Ayarlarý")]
    public float hoverScale = 1.15f;
    public float animDuration = 0.15f;
    public Color highlightColor = new Color(1f, 0.9f, 0.6f); // Sarýya yakýn

    void Awake()
    {
        keyData = GetComponent<KeyHintData>();
        originalScale = transform.localScale;

        image = GetComponent<Image>();
        if (image != null)
            originalColor = image.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(keyData.keyCode))
        {
            KeyHintManager.Instance.ShowHint(keyData.description);
            AnimateIn();
        }

        if (Input.GetKeyUp(keyData.keyCode))
        {
            KeyHintManager.Instance.HideHint();
            AnimateOut();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        KeyHintManager.Instance.ShowHint(keyData.description);
        AnimateIn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KeyHintManager.Instance.HideHint();
        AnimateOut();
    }

    private void AnimateIn()
    {
        transform.DOScale(originalScale * hoverScale, animDuration).SetEase(Ease.OutBack);

        if (image != null)
            image.DOColor(highlightColor, animDuration);
    }

    private void AnimateOut()
    {
        transform.DOScale(originalScale, animDuration).SetEase(Ease.InOutSine);

        if (image != null)
            image.DOColor(originalColor, animDuration);
    }
}
