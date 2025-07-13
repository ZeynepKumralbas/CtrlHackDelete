using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI statusText;
    public Button continueButton;

    void Start()
    {
        statusText.text = "Connecting to server...";
        continueButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings(); // Baðlantýyý baþlat
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected!";
        continueButton.interactable = true;
        continueButton.GetComponent<CanvasGroup>().DOFade(1f, 1f);
    }

    public void LoadLobby()
    {
        Debug.Log("Continue button clicked. Loading LobbyScene...");
        SceneManager.LoadScene("LobbyScene");

    }
}
