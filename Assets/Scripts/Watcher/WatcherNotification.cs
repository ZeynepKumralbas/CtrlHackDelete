using System.Collections;
using TMPro;
using UnityEngine;

public class WatcherNotification : MonoBehaviour
{
    public int notificationDelay = 1;
    public int notificationScreenTime = 5;

    private GameObject pnlNotification;

    private bool isNotifying = false; // Coroutine'in çalýþýp çalýþmadýðýný takip etmek için
    private bool previousMissionCompleted = false; // Daha önce tamamlanmýþ mýydý

    void Start()
    {
        pnlNotification = UIManager.Instance.pnlNotification;
    }

    void Update()
    {
        if (PlayerInteraction.Instance != null)
        {
            if (PlayerInteraction.Instance.isCompleted && !isNotifying && !previousMissionCompleted)
            {
                StartCoroutine(Notification_MissionComplete());
                isNotifying = true;
                previousMissionCompleted = true;
            }
            else if (!PlayerInteraction.Instance.isCompleted)
            {
                previousMissionCompleted = false;
            }
        }
    }

    IEnumerator Notification_MissionComplete()
    {
        string notificationMessage = "";

        if (PlayerInteraction.Instance.roomName != null)
        {
            notificationMessage = PlayerInteraction.Instance.roomName + " odasýnda birtakým sorunlar oluþtu";
        }

        Debug.Log("Delay Baþlangýç");
        yield return new WaitForSeconds(notificationDelay);
        Debug.Log("Delay bitiþ");

        Notification(true, notificationMessage);

        Debug.Log("Ekran süresi Baþlangýç");
        yield return new WaitForSeconds(notificationScreenTime);

        Notification(false, "");
        
        isNotifying = false;
        PlayerInteraction.Instance.isCompleted = false;
    }
    public void Notification(bool setVisibilty, string notificationText)
    {
        pnlNotification.SetActive(setVisibilty);
        pnlNotification.GetComponentInChildren<TextMeshProUGUI>().text = notificationText;
    }
}
