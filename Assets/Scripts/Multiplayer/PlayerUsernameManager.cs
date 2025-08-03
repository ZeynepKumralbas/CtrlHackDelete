using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerUsernameManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TextMeshProUGUI errorMessageText;

    void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
    }

    public void PlayerUsernameInputValueChanged()
    {
        string username = usernameInput.text;

        if (!string.IsNullOrEmpty(username) && username.Length <= 20)
        {
            PhotonNetwork.NickName = username;
            PlayerPrefs.SetString("username", username);
            Launcher.instance.hasEnteredUsernameThisSession = true;
            errorMessageText.text = "";
            MenuManager.instance.OpenMenu("TitleMenu");
        }
        else
        {
            errorMessageText.text = "Username must not be empty and should be 20 characters or less";
        }
    }
}
