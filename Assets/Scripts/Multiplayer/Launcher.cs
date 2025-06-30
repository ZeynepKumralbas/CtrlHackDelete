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
    public GameObject startButton;
    int nextTeamNumber = 1;
    public bool hasEnteredUsernameThisSession = false;
    bool returnToMenuScene = false;

    // Team selection
    public Transform humansTeamContent;
    public Transform watchersTeamContent;

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
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("RoomMenu");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            int teamNumber = GetNextTeamMember();
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i], "None");
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string errorMessage)
    {
        errorText.text = "Room Generation Unsuccesfull" + errorMessage;
        MenuManager.instance.OpenMenu("ErrorMenu");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public void StartGame()
    {
        Debug.Log("Start Game..");
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        MenuManager.instance.CloseAllMenus();
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

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        if (returnToMenuScene)
        {
            Debug.Log("Return to menu scene");
            //  GameObject obj = new GameObject("SceneLoader");
            StartCoroutine(LoadScene("Menu"));
            Destroy(RoomManager.instance.gameObject);
        }
        else
        {
            MenuManager.instance.OpenMenu("TitleMenu");
        }
        returnToMenuScene = false;
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
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //    int teamNumber = GetNextTeamMember();
        GameObject playerItem = Instantiate(playerListItemPrefab, playerListContent);
        playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer, "None");
        UpdatePlayerList();
    }

    private int GetNextTeamMember()
    {
        int teamMember = nextTeamNumber;
        nextTeamNumber = 3 - nextTeamNumber;
        return teamMember;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Team"))
        {
            UpdatePlayerList();

            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                RoomMenuUIController.instance.UpdateButtonVisibility();
            }
        }
    }
    
    void UpdatePlayerList()
    {

        Debug.Log("Update Player List");
        // Önce hepsini temizle
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

    public void QuitGame()
    {
        Debug.Log("GameLeaved");
        Application.Quit();
    }
}
