using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        roomNameText.text = _info.Name;
    }

    public void onClick()
    {
        Launcher.instance.JoinRoom(info);
    }
}
