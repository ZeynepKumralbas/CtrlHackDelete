using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MissionManager : MonoBehaviourPun
{
    [SerializeField] private int missionsPerPlayer = 0;

    private List<Transform> allMissions = new List<Transform>();

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CollectAllMissions();
            AssignMissionsToPlayers();
        }
    }

    private void CollectAllMissions()
    {
        foreach (Transform mission in transform)
        {
            allMissions.Add(mission);
            // Baþlangýçta tüm MissionSphere'leri devre dýþý býrak
            Transform sphere = mission.GetChild(0);
            if (sphere != null)
                sphere.gameObject.SetActive(false);
        }
    }

    private void AssignMissionsToPlayers()
    {
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);

        if(allMissions.Count % players.Count != 0)
        {
            for (int i = 0; i < allMissions.Count % players.Count; i++)
            {
                allMissions.Remove(allMissions[Random.Range(0,allMissions.Count)]);
            }
        }

        missionsPerPlayer = allMissions.Count / players.Count;
        if (allMissions.Count < players.Count)
        {
            Debug.LogWarning("Yeterli görev yok!");
            return;
        }

        List<Transform> shuffled = new List<Transform>(allMissions);
        ShuffleList(shuffled);

        int missionIndex = 0;

        foreach (Player player in players)
        {
            List<string> assignedMissionNames = new List<string>();

            for (int i = 0; i < missionsPerPlayer; i++)
            {
                Transform mission = shuffled[missionIndex++];
                assignedMissionNames.Add(mission.name); // MissionPoint_A, etc.
            }

            // RPC ile ilgili oyuncuya görevleri gönder
            photonView.RPC("ActivateMissionsForPlayer", player, assignedMissionNames.ToArray());
        }
    }

    [PunRPC]
    private void ActivateMissionsForPlayer(string[] missionNames)
    {
        foreach (string missionName in missionNames)
        {
            Transform mission = transform.Find(missionName);
            if (mission != null)
            {
                Transform sphere = mission.GetChild(0);
                if (sphere != null)
                    sphere.gameObject.SetActive(true);
            }
        }
    }

    // Liste karýþtýrma fonksiyonu
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
