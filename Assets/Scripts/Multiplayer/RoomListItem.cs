using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using UnityEngine.EventSystems;


public class RoomListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI roomNameText;
    public RoomInfo info;
    private string tooltipMessage = "";

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        roomNameText.text = _info.Name;

        Debug.Log("info:" + info);
        Debug.Log("infodan:" + info.CustomProperties["GameStarted"]);
        bool gameStarted = false;
        if (info.CustomProperties.ContainsKey("GameStarted"))
        {
            Debug.Log("has it");
            gameStarted = (bool)info.CustomProperties["GameStarted"];
        }

        Debug.Log("gameStarted:" + gameStarted);

        if (gameStarted)
        {
            GetComponent<Button>().interactable = false;
            tooltipMessage = "Game in progress 🔒";
        }
        else if (info.PlayerCount >= info.MaxPlayers)
        {
            GetComponent<Button>().interactable = false;
            tooltipMessage = "Full";
        }
        else
        {
            GetComponent<Button>().interactable = true;
            tooltipMessage = "";
        }
    }

    public void onClick()
    {
        Launcher.instance.JoinRoom(info);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(tooltipMessage))
        {
            TooltipManager.instance.ShowTooltip(tooltipMessage, gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }

}
