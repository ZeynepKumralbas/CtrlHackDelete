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
