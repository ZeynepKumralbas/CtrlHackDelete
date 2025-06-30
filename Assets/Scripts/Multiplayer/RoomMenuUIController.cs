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

    public void ChangeTeam(string newTeam)
    {
        string currentTeam = "None";
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            currentTeam = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
        }

        // Ortadayken Humans veya Watchers’a geçebilir
        if (currentTeam != "None" && newTeam != "None")
        {
            Debug.Log("Takım değiştirilemez: önce ortadaki alana dönmelisin.");
            return;
        }

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Team"] = newTeam;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void UpdateButtonVisibility()
    {
        string team = "None";
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            team = PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString();
        }

        bool isMiddle = team == "None";
        bool isWatchers = team == "Watchers";
        bool isHumans = team == "Humans";

        Debug.Log("team: " + team + "isMiddle: " + isMiddle);

        bufferToHumansButton.interactable = isMiddle;
        bufferToWatchersButton.interactable = isMiddle;
        humansToBufferButton.interactable = isHumans;
        watchersToBufferButton.interactable = isWatchers;
    }
}
