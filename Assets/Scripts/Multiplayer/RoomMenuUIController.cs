using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomMenuUIController : MonoBehaviourPunCallbacks
{
    public static RoomMenuUIController instance;

    [Header("Team Buttons")]
    public Button bufferToHumansButton;
    public Button bufferToWatchersButton;
    public Button humansToBufferButton;
    public Button watchersToBufferButton;

    private void Awake()
    {
        instance = this;

        // Button listener'ları burada bağla
        bufferToHumansButton.onClick.AddListener(() => ChangeTeam("Humans"));
        bufferToWatchersButton.onClick.AddListener(() => ChangeTeam("Watchers"));
        humansToBufferButton.onClick.AddListener(() => ChangeTeam("None"));
        watchersToBufferButton.onClick.AddListener(() => ChangeTeam("None"));
    }

    private void Start()
    {
        UpdateButtonVisibility();
    }

    /*    
        private void TryJoinWatchers()
        {
            int watchersCount = 0;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.CustomProperties.ContainsKey("Team") && p.CustomProperties["Team"]?.ToString() == "Watchers")
                {
                    watchersCount++;
                }
            }

            if (watchersCount >= 1)
            {
                Debug.Log("Watchers team already full!");
                return;
            }

            // Takıma geçiş
            ChangeTeam("Watchers");

            // Herkeste butonu kapat
            photonView.RPC("RPC_DisableWatchersButton", RpcTarget.All);
        }
        */

    public void ChangeTeam(string newTeam)
    {
        Debug.Log("Change Team");
        string currentTeam = "None";
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            currentTeam = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
        }

        // Can go to Humans or Watchers teams when in Buffer
        if (currentTeam != "None" && newTeam != "None")
        {
            Debug.Log("Cannot change the team. Go back to Buffer.");
            return;
        }

        /*
                // Check if Watchers team is full
                if (newTeam == "Watchers")
                {
                    int watchersCount = 0;
                    foreach (Player p in PhotonNetwork.PlayerList)
                    {
                        if (p.CustomProperties.ContainsKey("Team") && p.CustomProperties["Team"]?.ToString() == "Watchers")
                        {
                            watchersCount++;
                        }
                    }

                    if (watchersCount >= 1)
                    {
                        Debug.Log("Watchers team has already full!");
                        //    UpdateButtonVisibility();
                        return;
                    }
                }
        */
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Team"] = newTeam;
        Debug.Log("old team:" + currentTeam + " new team:" + newTeam);
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        UpdateButtonVisibility();
    }
/*
    [PunRPC]
    public void RPC_DisableWatchersButton()
    {
        bufferToWatchersButton.interactable = false;
    }*/
    /*
        public void UpdateButtonVisibility()
        {
            string team = "None";

            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            {
                team = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
                bool isMiddle = team == "None";
                bool isWatchers = team == "Watchers";
                bool isHumans = team == "Humans";

                Debug.Log("team: " + team + "isMiddle: " + isMiddle);

                bufferToHumansButton.interactable = isMiddle;
                bufferToWatchersButton.interactable = isMiddle;
                humansToBufferButton.interactable = isHumans;
                watchersToBufferButton.interactable = isWatchers;
            }
            else
            {
                int watchersCount = 0;
                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.CustomProperties.ContainsKey("Team") && p.CustomProperties["Team"]?.ToString() == "Watchers")
                    {
                        watchersCount++;
                    }
                }
                bool watchersAvailable = watchersCount == 0;
                bufferToWatchersButton.interactable = watchersAvailable;
            }
        }
        */
    public void UpdateButtonVisibility()
    {
        
        string localTeam = "None";
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            localTeam = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
        }
        Debug.Log($"Local Player Team: {localTeam}");

        // Watchers takımında kaç kişi var?
        int watchersCount = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.ContainsKey("Team") &&
                p.CustomProperties["Team"]?.ToString() == "Watchers")
            {
                watchersCount++;
            }
        }

        bool watchersAvailable = watchersCount == 0;

        // Buffer'da (None) olan ve watchers boşsa buton aktif olsun
        bool canJoinWatchers = (localTeam == "None") && watchersAvailable;

        // Humans takımındaki oyuncular watchers butonuna basamasın
        bufferToWatchersButton.interactable = canJoinWatchers;

        bufferToHumansButton.interactable = (localTeam == "None");
        humansToBufferButton.interactable = (localTeam == "Humans");
        watchersToBufferButton.interactable = (localTeam == "Watchers");
    }

}
