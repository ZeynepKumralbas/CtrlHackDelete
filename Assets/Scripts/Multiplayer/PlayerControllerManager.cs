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

    private GameObject humanUI;
    private GameObject watcherUI;

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
            UpdateUIBasedOnRole();
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
    /* UI Visibility Settings*/
    void UpdateUIBasedOnRole()
    {
        humanUI = GameObject.FindWithTag("HumanUI");
        watcherUI = GameObject.FindWithTag("WatcherUI");

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            string team = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();

            // Varsay�lan olarak t�m UI'leri kapat
            if (humanUI != null) humanUI.SetActive(false);
            if (watcherUI != null) watcherUI.SetActive(false);

            // Rol bazl� a��lacak olanlar� aktif et
            if (team == "Humans" && humanUI != null)
                humanUI.SetActive(true);
            else if (team == "Watchers" && watcherUI != null)
                watcherUI.SetActive(true);
        }
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
            //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { view.ViewID });
            if(team == "Watchers")
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Watcher"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { view.ViewID });
            else if(team == "Humans")
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { view.ViewID });
            
            Debug.Log("Instantiated Player Controller at spawn point");
            if (controller.GetComponent<PhotonView>().IsMine)
            {
                CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
                Debug.Log("cam:" + cam);
                cam.Follow = controller.transform;

                var brain = Camera.main.GetComponent<CinemachineBrain>();
                brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

                var mapCam = GameObject.Find("MapCam");

                /* Layer based object visibility setting*/
                if (team == "Watchers")
                {
                    brain.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("VisibleForHumans"));
                    mapCam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("VisibleForHumans"));
                }
                /**********************************************/
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
