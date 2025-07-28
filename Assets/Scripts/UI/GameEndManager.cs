/*using UnityEngine;
using UnityEngine.Playables;
using Photon.Pun;
using TMPro;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;

    [Header("Cutscenes")]
    public PlayableDirector humansWinCutscene;
    public PlayableDirector watcherWinCutscene;
    public PlayableDirector watcherLoseCutscene;
    public PlayableDirector timeUpCutscene;
    public PlayableDirector playerLeftCutscene;

    [Header("UI")]
    public GameObject gameEndPanel;
    public TMP_Text gameEndReasonText;
    public GameObject skipInstructionText;

    private PlayableDirector currentCutscene;
    public bool gameEnded = false;
    private bool cutsceneStarted = false;
    private bool exitScheduled = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!cutsceneStarted || exitScheduled)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Cutscene skipped by player.");
            SkipCutsceneAndExit();
        }
    }

    [PunRPC]
    public void RPC_EndGame(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;

        DisableAllPlayerControls();
        ShowEndPanel(reason);
        StartCoroutine(StartCutsceneAfterDelay(reason, 2f));
    }

    private void ShowEndPanel(string reason)
    {
        if (gameEndPanel != null && gameEndReasonText != null)
        {
            gameEndPanel.SetActive(true);

            switch (reason)
            {
                case "Humans Win":
                    gameEndReasonText.text = "🎉 Humans have completed all tasks and escaped!";
                    break;
                case "Watcher Wins":
                    gameEndReasonText.text = "👁️ Watcher has found all impostors!";
                    break;
                case "Watcher Defeated":
                    gameEndReasonText.text = "💥 Watcher is defeated by false smashes!";
                    break;
                case "Time Up":
                    gameEndReasonText.text = "⏰ Time's up! No clear winner.";
                    break;
                case "Player Left":
                    gameEndReasonText.text = "🚪 A player left the game. Match ended.";
                    return; // Cutscene oynatma!
                default:
                    gameEndReasonText.text = "Game ended.";
                    break;
            }
        }
    }

    private System.Collections.IEnumerator StartCutsceneAfterDelay(string reason, float delay)
    {
        yield return new WaitForSeconds(delay);

        gameEndPanel.SetActive(false);
        cutsceneStarted = true;

        if (skipInstructionText != null)
            skipInstructionText.SetActive(true);

        switch (reason)
        {
            case "Humans Win":
                currentCutscene = humansWinCutscene;
                break;
            case "Watcher Wins":
                currentCutscene = watcherWinCutscene;
                break;
            case "Watcher Defeated":
                currentCutscene = watcherLoseCutscene;
                break;
            case "Time Up":
                currentCutscene = timeUpCutscene;
                break;
        }

        if (currentCutscene != null)
        {
            currentCutscene.gameObject.SetActive(true);
            currentCutscene.stopped += OnCutsceneStopped;
            currentCutscene.Play();
        }
        else
        {
            Debug.LogWarning("No cutscene set for reason: " + reason);
            BackToMenu();
        }
    }

    private void OnCutsceneStopped(PlayableDirector dir)
    {
        if (!exitScheduled)
        {
            Debug.Log("Cutscene finished naturally.");
            BackToMenu();
        }
    }

    private void SkipCutsceneAndExit()
    {
        exitScheduled = true;

        if (skipInstructionText != null)
            skipInstructionText.SetActive(false);

        if (currentCutscene != null && currentCutscene.state == PlayState.Playing)
        {
            currentCutscene.Stop();
        }

        BackToMenu();
    }

    private void BackToMenu()
    {
        exitScheduled = true;

        if (skipInstructionText != null)
            skipInstructionText.SetActive(false);

        Launcher.instance.returnToMenuScene = true;
        PhotonNetwork.LeaveRoom();
    }

    void DisableAllPlayerControls()
    {
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            if (!pv.IsMine) continue;

            GameObject go = pv.gameObject;

            var move = go.GetComponent<PlayerMovement>();
            var interact = go.GetComponent<PlayerInteraction>();
            var humanDeath = go.GetComponent<PlayerDeath>();
            var humanSkill = go.GetComponent<PlayerSkills>();

            var watcherDeath = go.GetComponent<WatcherDeath>();
            var watcherEffectedFromPlayerSkills = go.GetComponent<WatcherEffectedFromPlayerSkills>();
            var watcherInteraction = go.GetComponent<WatcherInteraction>();
            var watcherNotification = go.GetComponent<WatcherNotification>();
            var watcherSmash = go.GetComponent<WatcherSmash>();

            if (move != null) move.enabled = false;
            if (interact != null) interact.enabled = false;
            if (humanDeath != null) humanDeath.enabled = false;
            if (humanSkill != null) humanSkill.enabled = false;

            if (watcherDeath != null) watcherDeath.enabled = false;
            if (watcherEffectedFromPlayerSkills != null) watcherEffectedFromPlayerSkills.enabled = false;
            if (watcherInteraction != null) watcherInteraction.enabled = false;
            if (watcherNotification != null) watcherNotification.enabled = false;
            if (watcherSmash != null) watcherSmash.enabled = false;
        }
    }

}
*/

/*
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;

    [Header("UI")]
    public GameObject gameEndPanel;
    public TMP_Text gameEndReasonText;

    public bool gameEnded = false;

    private void Awake()
    {
        Instance = this;
    }

    [PunRPC]
    public void RPC_EndGame(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;

        switch (reason)
        {
            case "Humans Win":
                LoadCutsceneScene("Cutscene_HumansWin");
                return;

            case "Watcher Wins":
                LoadCutsceneScene("WatcherWin");
                return;

            case "Watcher Defeated":
                LoadCutsceneScene("WatcherLose");
                return;

            case "Time Up":
                LoadCutsceneScene("TimeIsUp");
                return;

            case "Player Left":
                ShowEndPanel("🚪 A player left the game. Match ended.");
                Invoke(nameof(BackToMenu), 3f);
                return;

            default:
                ShowEndPanel("Game ended.");
                Invoke(nameof(BackToMenu), 3f);
                return;
        }
    }

    private void LoadCutsceneScene(string sceneName)
    {
        // Bu sahnelerde direkt Timeline otomatik oynatılır
        PhotonNetwork.LoadLevel(sceneName);
    }

    private void ShowEndPanel(string message)
    {
        if (gameEndPanel != null && gameEndReasonText != null)
        {
            gameEndPanel.SetActive(true);
            gameEndReasonText.text = message;
        }
    }

    private void BackToMenu()
    {
        Launcher.instance.returnToMenuScene = true;
        PhotonNetwork.LeaveRoom();
    }

}

*/

using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;

    [Header("UI")]
    public GameObject gameEndPanel;
    public TMP_Text gameEndReasonText;
    public Button nextButton;

    public bool gameEnded = false;
    private string cutsceneSceneToLoad = "";

    private void Awake()
    {
        Instance = this;

        // Buton setup
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
            nextButton.gameObject.SetActive(false); // Başta gizli dursun
        }
    }

    [PunRPC]
    public void RPC_EndGame(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;

        string message = "";

        switch (reason)
        {
            case "HumansWin":
                message = "🎉 Humans have completed all tasks and escaped!";
                cutsceneSceneToLoad = "HumansWin";
                break;

            case "WatcherWin":
                message = "👁️ Watcher has found all impostors!";
                cutsceneSceneToLoad = "WatcherWin";
                break;

            case "WatcherLose":
                message = "💥 Watcher is defeated by false smashes!";
                cutsceneSceneToLoad = "WatcherLose";
                break;

            case "TimeIsUp":
                message = "⏰ Time's up! No clear winner.";
                cutsceneSceneToLoad = "TimeIsUp";
                break;

            case "PlayerLeft":
                message = "🚪 A player left the game. Match ended.";
                cutsceneSceneToLoad = ""; // Cutscene yok
                Invoke(nameof(BackToMenu), 3f);
                break;

            default:
                message = "Game ended.";
                Invoke(nameof(BackToMenu), 3f);
                break;
        }

        ShowEndPanel(message);
        DisableAllPlayerControls();
    }

    private void ShowEndPanel(string message)
    {
        if (gameEndPanel != null && gameEndReasonText != null)
        {
            gameEndPanel.SetActive(true);
            gameEndReasonText.text = message;

            if (!string.IsNullOrEmpty(cutsceneSceneToLoad) && nextButton != null)
            {
                nextButton.gameObject.SetActive(true);
            }
        }
    }

    private void OnNextButtonClicked()
    {
        if (!string.IsNullOrEmpty(cutsceneSceneToLoad))
        {
            PhotonNetwork.LoadLevel(cutsceneSceneToLoad);
        }
    }

    private void BackToMenu()
    {
        Launcher.instance.returnToMenuScene = true;
        PhotonNetwork.LeaveRoom();
    }

    void DisableAllPlayerControls()
    {
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            if (!pv.IsMine) continue;

            GameObject go = pv.gameObject;

            var move = go.GetComponent<PlayerMovement>();
            var interact = go.GetComponent<PlayerInteraction>();
            var humanDeath = go.GetComponent<PlayerDeath>();
            var humanSkill = go.GetComponent<PlayerSkills>();

            var watcherDeath = go.GetComponent<WatcherDeath>();
            var watcherEffectedFromPlayerSkills = go.GetComponent<WatcherEffectedFromPlayerSkills>();
            var watcherInteraction = go.GetComponent<WatcherInteraction>();
            var watcherNotification = go.GetComponent<WatcherNotification>();
            var watcherSmash = go.GetComponent<WatcherSmash>();

            if (move != null) move.enabled = false;
            if (interact != null) interact.enabled = false;
            if (humanDeath != null) humanDeath.enabled = false;
            if (humanSkill != null) humanSkill.enabled = false;

            if (watcherDeath != null) watcherDeath.enabled = false;
            if (watcherEffectedFromPlayerSkills != null) watcherEffectedFromPlayerSkills.enabled = false;
            if (watcherInteraction != null) watcherInteraction.enabled = false;
            if (watcherNotification != null) watcherNotification.enabled = false;
            if (watcherSmash != null) watcherSmash.enabled = false;
        }
    }
}
