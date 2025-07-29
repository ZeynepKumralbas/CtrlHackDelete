using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBarColorChanger : MonoBehaviour
{
    [Header("Referanslar")]
    public Slider healthSlider;
    public Image fillImage;
    public Transform healthBarTransform;
    public Image blinkBackgroundImage; // arkaplan i�in ayr� bir g�rsel (iste�e ba�l�)

    [Header("Renk Ayarlar�")]
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    [Header("E�ik De�erleri")]
    [Range(0f, 1f)] public float mediumThreshold = 0.5f;
    [Range(0f, 1f)] public float lowThreshold = 0.2f;

    [Header("Animasyon")]
    public float punchScale = 0.2f;
    public float punchDuration = 0.3f;

    [Header("Blink")]
    public bool enableBlink = true;
    public float blinkSpeed = 4f;
    public Color blinkColor = new Color(1f, 0f, 0f, 0.3f); // yar� saydam k�rm�z�

    private float lastHealthValue;
    private Color originalBlinkColor;

    void Start()
    {
        lastHealthValue = healthSlider.value;

        if (blinkBackgroundImage != null)
        {
            originalBlinkColor = blinkBackgroundImage.color;
        }
    }

    void Update()
    {
        float healthPercent = healthSlider.value / healthSlider.maxValue;

        // Renk ge�i�i
        Color targetColor = GetColorForHealth(healthPercent);
        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * 10f);

        // Can azald���nda animasyon
        if (healthSlider.value < lastHealthValue)
        {
            AnimatePunch();
        }

        // Blink efekti (can �ok d���kse)
        if (enableBlink && blinkBackgroundImage != null)
        {
            if (healthPercent < lowThreshold)
            {
                float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; // 0�1
                Color c = Color.Lerp(Color.clear, blinkColor, alpha);
                blinkBackgroundImage.color = c;
            }
            else
            {
                blinkBackgroundImage.color = Color.Lerp(blinkBackgroundImage.color, originalBlinkColor, Time.deltaTime * 5f);
            }
        }

        lastHealthValue = healthSlider.value;
    }

    Color GetColorForHealth(float percent)
    {
        if (percent > mediumThreshold) return highHealthColor;
        else if (percent > lowThreshold) return mediumHealthColor;
        else return lowHealthColor;
    }

    void AnimatePunch()
    {
        if (healthBarTransform != null)
        {
            healthBarTransform.DOKill();
            healthBarTransform.localScale = Vector3.one;
            healthBarTransform.DOPunchScale(Vector3.one * punchScale, punchDuration, 6, 1);
        }
    }
}
