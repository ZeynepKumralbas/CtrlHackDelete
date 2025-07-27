using System.Collections;
using TMPro;
using UnityEngine;

public class WatcherNotification : MonoBehaviour
{
    public int notificationDelay = 1;
    public int notificationScreenTime = 5;

    private GameObject pnlNotification;

    private bool isNotifying = false; // Coroutine'in �al���p �al��mad���n� takip etmek i�in
    private bool previousMissionCompleted = false; // Daha �nce tamamlanm�� m�yd�

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
            notificationMessage = PlayerInteraction.Instance.roomName + " odas�nda birtak�m sorunlar olu�tu";
        }

        Debug.Log("Delay Ba�lang��");
        yield return new WaitForSeconds(notificationDelay);
        Debug.Log("Delay biti�");

        Notification(true, notificationMessage);

        Debug.Log("Ekran s�resi Ba�lang��");
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
