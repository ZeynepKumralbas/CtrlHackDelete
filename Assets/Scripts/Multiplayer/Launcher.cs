using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public Button loadingNextButton;
    public TMP_InputField roomNameInputField;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI errorText;
    public Transform roomListContent;
    public GameObject roomListItemPrefab;
    public Transform playerListContent;
    public GameObject playerListItemPrefab;
    public Button startButton;
    int nextTeamNumber = 1;
    public bool hasEnteredUsernameThisSession = false;
    public bool returnToMenuScene = false;

    // Team selection
    public Transform humansTeamContent;
    public Transform watchersTeamContent;

    // room cache
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> roomUIElements = new Dictionary<string, GameObject>();


    /*   void Awake()
       {
           instance = this;
       }
   */
    void Awake()
    {
        //  instance = this;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master...");
        PhotonNetwork.GameVersion = "1.0"; // versiyonlar farklıysa oyuncular birbirini göremez
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu"; // <- Region burada ayarlanıyor
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        //MenuManager.instance.OpenMenu("UsernameMenu");
        Debug.Log("Joined Lobby");
        if (!hasEnteredUsernameThisSession)
        {
            loadingNextButton.gameObject.SetActive(true);
            //    MenuManager.instance.OpenMenu("UsernameMenu");
        }
        else
        {
            MenuManager.instance.OpenMenu("TitleMenu");
        }
        startButton.interactable = false;
        UpdateStartButtonState();

        cachedRoomList.Clear();
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
        customProps["GameStarted"] = false;

        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 5,
            CustomRoomProperties = customProps,
            CustomRoomPropertiesForLobby = new string[] { "GameStarted" }
        };

        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("RoomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

    //    foreach (Transform child in playerListContent)
    //    {
    //        Destroy(child.gameObject);
    //    }

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Team"] = "None";
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        //    for (int i = 0; i < players.Count(); i++)
        //    {
        //    int teamNumber = GetNextTeamMember();
        //        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i], "None");
        //    }
        UpdatePlayerList();
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string errorMessage)
    {
        errorText.text = "Room Generation Unsuccesfull" + errorMessage;
        MenuManager.instance.OpenMenu("ErrorMenu");
    }

    public void JoinRoom(RoomInfo info)
    {
         if (info.PlayerCount >= info.MaxPlayers)
        {
            Debug.Log($"Room '{info.Name}' is full.");
            errorText.text = $"Room '{info.Name}' is full. You cannot join.";
            MenuManager.instance.OpenMenu("ErrorMenu");
            return;
        }

        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public void StartGame()
    {
        Debug.Log("Start Game..");

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = true;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["GameStarted"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        MenuManager.instance.CloseAllMenus();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GameStarted"))
        {
            Debug.Log("GameStarted property updated: " + propertiesThatChanged["GameStarted"]);
        }
    }

    public void LeaveRoom()
    {
        Debug.Log("Launcher LeaveRoom");

        Debug.Log($"InRoom: {PhotonNetwork.InRoom}, IsConnected: {PhotonNetwork.IsConnected}");

        //    MenuManager.instance.OpenMenu("LoadingMenu");
        if (SceneManager.GetActiveScene().name == "Game")
        {
            returnToMenuScene = true;
        }


        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        // PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        //    PhotonNetwork.JoinLobby();

        if (returnToMenuScene)
        {
            Debug.Log("Return to menu scene");
            //  GameObject obj = new GameObject("SceneLoader");
            PhotonNetwork.LoadLevel("Menu");
        //    StartCoroutine(LoadScene("Menu"));
            Destroy(RoomManager.instance.gameObject);
        }
        else
        {
            MenuManager.instance.OpenMenu("TitleMenu");
        }
        returnToMenuScene = false;

        cachedRoomList.Clear();
    }

    private IEnumerator LoadScene(string scene)
    {
        Debug.Log("coroutine");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room List Updated:");
        /*
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        Debug.Log("roomList.Count:" + roomList.Count);
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log($"Room: {roomList[i].Name} | Open: {roomList[i].IsOpen} | Visible: {roomList[i].IsVisible} | Removed: {roomList[i].RemovedFromList}");
            if (roomList[i].RemovedFromList)
            {
                Debug.Log(roomList[i].Name + "already removed, continue");
                continue;
            }
            Debug.Log("Instantiate i:" + i + " name:" + roomList[i].Name);
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }*/

        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList || //!info.IsOpen ||
            !info.IsVisible || info.PlayerCount == 0)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    Debug.Log($"Removing invalid/closed room: {info.Name}");
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                cachedRoomList[info.Name] = info;
                if (info.CustomProperties != null && info.CustomProperties.ContainsKey("GameStarted"))
                {
                    Debug.Log($"Room {info.Name} GameStarted: {info.CustomProperties["GameStarted"]}");
                }
            }
        }

        UpdateRoomListUI();
    }

    private void UpdateRoomListUI()
    {
        /*
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        foreach (RoomInfo roomInfo in cachedRoomList.Values)
        {
            // Oda gerçekten aktif mi?
            if (!roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.PlayerCount == 0)
            {
                Debug.Log($"Room {roomInfo.Name} is invalid. Skipping.");
                continue;
            }

            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomInfo);
        }*/
        

        // Check rooms
        foreach (var pair in cachedRoomList)
        {
            string roomName = pair.Key;
            RoomInfo info = pair.Value;

            if (roomUIElements.ContainsKey(roomName))
            {
                // UI already exists, update
                roomUIElements[roomName].GetComponent<RoomListItem>().SetUp(info);
            }
            else
            {
                // Create new UI
                GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
                roomItem.GetComponent<RoomListItem>().SetUp(info);
                roomUIElements[roomName] = roomItem;
            }
        }

        // Remove UI of deleted rooms
        List<string> toRemove = new List<string>();
        foreach (var kvp in roomUIElements)
        {
            if (!cachedRoomList.ContainsKey(kvp.Key))
            {
                if (TooltipManager.instance.IsShowingTooltipFor(kvp.Value))
                {
                    TooltipManager.instance.HideTooltip();
                }
                Destroy(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (string key in toRemove)
        {
            roomUIElements.Remove(key);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //    int teamNumber = GetNextTeamMember();
        //    GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);
        //    playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer, "None");
        UpdatePlayerList();
        UpdateStartButtonState();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
        RoomMenuUIController.instance.UpdateButtonVisibility();
        UpdateStartButtonState();


        Debug.Log("OnPlayerLeftRoom");
        // if one player leaves the game, end it for all
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("GameStarted", out object startedObj)
            && startedObj is bool started && started)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null && gm.photonView != null)
            {
                gm.photonView.RPC("RPC_ShowExitReasonAndEnd", RpcTarget.All, "One player has left the game.");
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        /*
        if (changedProps.ContainsKey("Team"))
        {
            UpdatePlayerList();

            //    if (targetPlayer == PhotonNetwork.LocalPlayer)
            //    {
            RoomMenuUIController.instance.UpdateButtonVisibility();
            //    }
        }
        */
        if (changedProps.ContainsKey("Team"))
        {
            RoomMenuUIController.instance.UpdateButtonVisibility();
            UpdatePlayerList();
            UpdateStartButtonState();
        }
    }
    
    void UpdatePlayerList()
    {

        Debug.Log("Update Player List");
        // Clear all
        foreach (Transform t in humansTeamContent) Destroy(t.gameObject);
        foreach (Transform t in watchersTeamContent) Destroy(t.gameObject);
        foreach (Transform t in playerListContent) Destroy(t.gameObject);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log("player: " + player);
            string team = "None";
            if (player.CustomProperties.ContainsKey("Team"))
            {
                Debug.Log("Team var:" + player.CustomProperties["Team"].ToString());
                team = player.CustomProperties["Team"].ToString();
            }

            GameObject item = Instantiate(playerListItemPrefab);

            switch (team)
            {
                case "Humans":
                    item.transform.SetParent(humansTeamContent, false);
                    Debug.Log("Humans");
                    break;
                case "Watchers":
                    item.transform.SetParent(watchersTeamContent, false);
                    Debug.Log("Watchers");
                    break;
                default:
                    item.transform.SetParent(playerListContent, false);
                    Debug.Log("None");
                    break;
            }

            item.GetComponent<PlayerListItem>().SetUp(player, team);
        }
    }

    public void UpdateStartButtonState()
    {
        int humansCount = 0;
        int watchersCount = 0;
        int bufferCount = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("Team", out object teamObj) && teamObj is string team)
            {
                switch (team)
                {
                    case "Humans":
                        humansCount++;
                        break;
                    case "Watchers":
                        watchersCount++;
                        break;
                    case "None":
                        bufferCount++;
                        break;
                }
            }
            else
            {
                bufferCount++; // If no team is selected, count as None
            }
        }

        bool canStart = PhotonNetwork.IsMasterClient &&
                        watchersCount == 1 &&
                        humansCount >= 1 &&
                        bufferCount == 0;

        startButton.interactable = canStart;
    }

    public void QuitGame()
    {
        Debug.Log("GameLeaved");
        Application.Quit();
    }
}
