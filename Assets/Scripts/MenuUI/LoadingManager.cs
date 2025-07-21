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
        PhotonNetwork.ConnectUsingSettings(); // Ba�lant�y� ba�lat
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected!";

        var cg = continueButton.GetComponent<CanvasGroup>();

        // H�zl� ve do�rudan g�r�n�r ve etkile�imli yap
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        continueButton.interactable = true;
    }

    public void LoadLobby()
    {
        Debug.Log("Continue button clicked. Loading LobbyScene...");
        SceneManager.LoadScene("LobbyScene");

    }
}
