using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Cinemachine;
using System;

public class PlayerControllerManager : MonoBehaviourPunCallbacks
{
    PhotonView view;
    public String playerTeam;
    GameObject controller;
    private Dictionary<int, int> playerTeams = new Dictionary<int, int>();

    void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (view.IsMine)
        {
            CreateController();
        }
    }



    void CreateController()
    {
        Debug.Log("CreateController");
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            playerTeam = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
            Debug.Log("Player's Team: " + playerTeam);
        }

        AssignPlayerToSpawnArea(playerTeam);
    }

    void AssignPlayerToSpawnArea(String team)
    {
        Debug.Log("AssignPlayerToSpawnArea");
        GameObject spawnAreaWatcher = GameObject.Find("SpawnAreaWatcher");
        GameObject spawnAreaImposters = GameObject.Find("SpawnAreaImposters");

        if (spawnAreaWatcher == null || spawnAreaImposters == null)
        {
            Debug.LogError("Spawn area not found");
            return;
        }

        Transform spawnPoint = null;

        if (team == "Watchers")
        {
            spawnPoint = spawnAreaWatcher.transform;
        }

        else if (team == "Humans")
        {
            spawnPoint = spawnAreaImposters.transform.GetChild(UnityEngine.Random.Range(0, spawnAreaImposters.transform.childCount));
        }

        if (spawnPoint != null)
        {
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { view.ViewID });
            Debug.Log("Instantiated Player Controller at spawn point");
            if (controller.GetComponent<PhotonView>().IsMine)
            {
                CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
                cam.Follow = controller.transform;

                var brain = Camera.main.GetComponent<CinemachineBrain>();
                brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
            }
        }
        else
        {
            Debug.LogError("No available spawn points for team " + team);
        }
    }
/*
    void AssignTeamsToAllPlayers()
    {
        Debug.Log("AssignTeamsToAllPlayers");
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team"))
            {
                int team = (int)player.CustomProperties["Team"];
                playerTeams[player.ActorNumber] = team;
                Debug.Log(player.NickName + "'s Team: " + team);

                AssignPlayerToSpawnArea(team);
            }
        }
    }
*/
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newplayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
    //    AssignTeamsToAllPlayers();
    }
}
