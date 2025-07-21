using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    public GameObject tooltipPrefab;
    private GameObject tooltipInstance;
    private RectTransform canvasRectTransform;
    private RectTransform tooltipRectTransform;
    private TextMeshProUGUI tooltipText;
    private GameObject currentSourceObject;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        canvasRectTransform = GetComponent<RectTransform>();

        tooltipInstance = Instantiate(tooltipPrefab, transform);
        tooltipRectTransform = tooltipInstance.GetComponent<RectTransform>();
        tooltipText = tooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
        tooltipInstance.SetActive(false);
    }

    void Update()
    {
        if (tooltipInstance.activeSelf)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null, out localPoint);

            Vector2 offset = new Vector2(50f, -50f); // biraz daha uzak
            Vector2 pos = localPoint + offset;

            // Canvas sınırlarını alalım
            Vector2 canvasSize = canvasRectTransform.sizeDelta;
            Vector2 tooltipSize = tooltipRectTransform.sizeDelta;

            // Sağ ve üst sınır kontrolü
            if (pos.x + tooltipSize.x > canvasSize.x / 2)
                pos.x = canvasSize.x / 2 - tooltipSize.x;

            if (pos.y - tooltipSize.y < -canvasSize.y / 2)
                pos.y = -canvasSize.y / 2 + tooltipSize.y;

            tooltipRectTransform.anchoredPosition = pos;
        }
    }

    public void ShowTooltip(string message, GameObject source = null)
    {
        Debug.Log("tooltip: " + message);
        tooltipText.text = message;
        tooltipInstance.SetActive(true);
        currentSourceObject = source;  // save the source
    }

    public void HideTooltip()
    {
        tooltipInstance.SetActive(false);
        currentSourceObject = null;  // remove the source    
    }

    public bool IsShowingTooltipFor(GameObject obj)
    {
        return tooltipInstance.activeSelf && currentSourceObject == obj;
    }
    
}
