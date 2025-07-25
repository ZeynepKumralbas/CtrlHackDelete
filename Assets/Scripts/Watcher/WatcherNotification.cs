using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WatcherNotification : MonoBehaviour
{
    public int notificationDelay = 1;
    public int notificationScreenTime = 5;

    private GameObject pnlNotification;
    void Start()
    {
        pnlNotification = UIManager.Instance.pnlNotification;
    }
    void Update()
    {
        if (PlayerInteraction.Instance.isCompleted)
        {
            StartCoroutine(Notification_MissionComplete());
            StopCoroutine(Notification_MissionComplete());
        }
        /*if (NPCde�i�keni)
        {
            StartCoroutine(Notification_OneMissionTwoRobot());
        }*/
    }

    IEnumerator Notification_MissionComplete()
    {
        string notificationMessage = PlayerInteraction.Instance.roomTag + " odas�nda birtak�m sorunlar olu�tu";

        Debug.Log("Delay Ba�lang��");
        yield return new WaitForSeconds(notificationDelay);
        Debug.Log("Delay biti�");
        Notification(true, notificationMessage);

        Debug.Log("Ekran s�resi Ba�lang��");
        yield return new WaitForSeconds(notificationScreenTime);
        Notification(false, "");

    }
    /*IEnumerator Notification_OneMissionTwoRobot()
    {
        Notification();
        yield return new WaitForSeconds(notificationScreenTime);
        Notification();
    }*/
    public void Notification(bool setVisibilty, string notificationText)
    {
        pnlNotification.SetActive(setVisibilty);
        pnlNotification.GetComponentInChildren<TextMeshProUGUI>().text = notificationText;
    }
}
