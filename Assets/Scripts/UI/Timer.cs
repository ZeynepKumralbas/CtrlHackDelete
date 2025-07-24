using System;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Timer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;

    private double startTime;
    private double roundDuration = 600.0; // 10 dakika
    private bool timerStarted = false;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("StartTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
            timerStarted = true;
        }
    }

    void Update()
    {
        if (!timerStarted)
            return;

        double elapsed = PhotonNetwork.Time - startTime;
        double remaining = roundDuration - elapsed;

        if (remaining <= 0)
        {
            remaining = 0;
            // Opsiyonel: süre bittiğinde bir şey yap
            // GameManager.Instance.EndGame();
        }

        UpdateTimerText(remaining);
    }

    private void UpdateTimerText(double timeRemaining)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeRemaining);
        timerText.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (!timerStarted && propertiesThatChanged.ContainsKey("StartTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
            timerStarted = true;
        }
    }

}
