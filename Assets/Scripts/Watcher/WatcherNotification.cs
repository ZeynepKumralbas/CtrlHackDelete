/*using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RoomStatusUI
{
    public string roomName;
    public Image circle;
}


public class WatcherNotification : MonoBehaviour
{
    public int notificationDelay = 1;
    public int notificationScreenTime = 5;

    private GameObject pnlNotification;

    private bool isNotifying = false; // Coroutine'in �al���p �al��mad���n� takip etmek i�in
    private bool previousMissionCompleted = false; // Daha �nce tamamlanm�� m�yd�

    public List<RoomStatusUI> roomStatuses;

    public static WatcherNotification Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        pnlNotification = UIManager.Instance.pnlNotification;
    }
    /*
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
    */
    /*   IEnumerator Notification_MissionComplete()
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
   */

 /*   IEnumerator Notification_MissionComplete(string roomName)
    {
        string notificationMessage = roomName + " odasında bazı sorunlar oluştu";

        yield return new WaitForSeconds(notificationDelay);

        Notification(true, notificationMessage);

        yield return new WaitForSeconds(notificationScreenTime);

        Notification(false, "");
    }

/*    public void ShowNotification(string roomName)
    {
        StartCoroutine(Notification_MissionComplete(roomName));
    }
*/
/*    public void Notification(bool setVisibilty, string notificationText)
    {
        pnlNotification.SetActive(setVisibilty);
        pnlNotification.GetComponentInChildren<TextMeshProUGUI>().text = notificationText;
    }

    public void ShowNotification(string roomName)
    {
        StartCoroutine(FlashRoomIndicator(roomName));
    }

    private IEnumerator FlashRoomIndicator(string roomName)
    {
        RoomStatusUI roomUI = roomStatuses.FirstOrDefault(r => r.roomName == roomName);

        if (roomUI != null && roomUI.circle != null)
        {
            float duration = notificationScreenTime;
            float elapsed = 0f;
            float flashInterval = 0.5f; // her yarım saniyede bir değiştir

            Color red = new Color(1, 0, 0, 1);
            Color white = new Color(1, 1, 1, 1);

            bool isRed = true;

            while (elapsed < duration)
            {
                roomUI.circle.color = isRed ? red : white;
                isRed = !isRed;

                yield return new WaitForSeconds(flashInterval);
                elapsed += flashInterval;
            }

            // Süre bitince beyaza sabitle
            roomUI.circle.color = white;
        }
        else
        {
            Debug.LogWarning("Room UI bulunamadı: " + roomName);
        }
    }

}
*/


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RoomStatusUI
{
    public string roomName;
    public Image circle;
}

public class WatcherNotification : MonoBehaviour
{
    public int notificationDelay = 1;
    public int notificationScreenTime = 5;


    public GameObject pnlNotification;

    private bool isNotifying = false;
    private string lastRoomNotified = "";

    public List<RoomStatusUI> roomStatuses;

    public static WatcherNotification Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
    //    pnlNotification = UIManager.Instance.pnlNotification;
        pnlNotification.SetActive(false); // Başta kapalı olmalı
    }

    /// <summary>
    /// Oda ismiyle birlikte bildirim başlatır.
    /// Aynı anda birden fazla bildirim gösterilmesini engeller.
    /// </summary>
    public void ShowNotification(string roomName)
    {
        if (!isNotifying)
        {
            StartCoroutine(Notification_MissionComplete(roomName));
        }
    }

    /// <summary>
    /// Bildirimi ekranda gösterir, süre sonunda kapatır.
    /// </summary>
    private IEnumerator Notification_MissionComplete(string roomName)
    {
        WatcherAudioManager.Instance.PlayAudioClip("notificationSound");
        isNotifying = true;

        string notificationMessage = $"Something is wrong in {roomName}";

        yield return new WaitForSeconds(notificationDelay);

        Notification(true, notificationMessage);

        yield return new WaitForSeconds(notificationScreenTime);

        Notification(false, "");

        isNotifying = false;
    }

    /// <summary>
    /// UI paneli açar veya kapar, mesajı ayarlar.
    /// </summary>
    private void Notification(bool setVisibility, string notificationText)
    {
        if (pnlNotification != null)
        {
            pnlNotification.SetActive(setVisibility);
            TextMeshProUGUI textComponent = pnlNotification.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = notificationText;
            }
        }
    }
}
