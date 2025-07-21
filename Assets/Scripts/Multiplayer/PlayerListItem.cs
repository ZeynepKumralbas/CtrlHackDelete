using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerUserName;
   // public TextMeshProUGUI teamText;
    Player player;
    int team;

    public void SetUp(Player _player, string _team)
    {
        player = _player;
        //   team = _team;
        playerUserName.text = _player.NickName;
     //   teamText.text = "Team " + _team;

        //    ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        //    customProps["Team"] = _team;
        //    _player.SetCustomProperties(customProps);
     //   teamText.text = string.IsNullOrEmpty(_team) ? "None" : _team;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
/*
    public void ChangeTeam(string team)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Team"] = team;
        Debug.Log("Change Team:" + PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    */
}
