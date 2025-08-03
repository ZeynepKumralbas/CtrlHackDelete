/*using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherEffectedFromPlayerSkills : MonoBehaviour
{
    public static WatcherEffectedFromPlayerSkills Instance;

    private PlayerSkills _playerSkills;

    public PhotonView view;
    private void Start()
    {
        Instance = this;
        _playerSkills = FindObjectOfType<PlayerSkills>();
    }

    public void SkillCooldown(int index, float cooldown)
    {
        _playerSkills.txtTimersForSkills[index].enabled = true;
        _playerSkills.activeCooldownsForSkills[index] = cooldown;

    }
    /*****************************************************************/
/*   [PunRPC]
   public void Skill_WatcherFreeze_Start(float cooldown)
   {
       WatcherAudioManager.Instance.PlayAudioClip("skill_FreezeEffectSound");

       GetComponent<PlayerMovement>().enabled = false;
       GetComponent<WatcherSmash>().enabled = false;
       transform.Find("Ice").gameObject.SetActive(true);

       SkillCooldown(0, cooldown);

       StartCoroutine(FreezeDurationCoroutine(cooldown));
   }
   private IEnumerator FreezeDurationCoroutine(float cooldown)
   {
       yield return new WaitForSeconds(cooldown);
       view.RPC("Skill_WatcherFreeze_End", RpcTarget.All); // <-- HERKESE G�NDER!
   }

   [PunRPC]
   public void Skill_WatcherFreeze_End()
   {
       GetComponent<PlayerMovement>().enabled = true;
       GetComponent<WatcherSmash>().enabled = true;
       transform.Find("Ice").gameObject.SetActive(false);

       _playerSkills.txtTimersForSkills[0].enabled = false;

       Debug.Log("Freeze sonland�");
   }

   /***************************************************************/
/*  [PunRPC]
  public void Skill_WatcherVisibilityBlock_Start(float cooldown)
  {
      WatcherAudioManager.Instance.PlayAudioClip("skill_CloseSightEffectSound");
      UIManager.Instance.pnlWatcherVisibilityBlocker.gameObject.SetActive(true);
      SkillCooldown(1, cooldown);
      StartCoroutine(VisibilityBlockDurationCoroutine(cooldown));
  }
  private IEnumerator VisibilityBlockDurationCoroutine(float cooldown)
  {
      yield return new WaitForSeconds(cooldown);
      view.RPC("Skill_WatcherVisibilityBlock_End", RpcTarget.All); // HERKESE!
  }

  [PunRPC]
  public void Skill_WatcherVisibilityBlock_End()
  {
      UIManager.Instance.pnlWatcherVisibilityBlocker.gameObject.SetActive(false);
      Debug.Log("Visibility block sonland�");
  }

}
*/

/*
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class WatcherEffectedFromPlayerSkills : MonoBehaviourPun
{
    public static WatcherEffectedFromPlayerSkills Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }


    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object role))
        {
            if (role.ToString() == "Watchers" && photonView.IsMine)
            {
                Debug.Log("Watcher start, kayıt coroutine başlıyor.");
                StartCoroutine(DelayedRegisterToHuman());
            }
        }
    }

    private IEnumerator DelayedRegisterToHuman()
    {
        yield return new WaitForSeconds(0.5f);

        PlayerSkills[] allSkills = FindObjectsOfType<PlayerSkills>();
        foreach (var skill in allSkills)
        {
        //    if (skill.photonView.IsMine)
        //    {
                Debug.Log("Watcher, human'a RegisterWatcher RPC gönderiyor.");
                skill.photonView.RPC("RegisterWatcher", skill.photonView.Owner, photonView.ViewID);
                break;
        //    }
        }
    }

    [PunRPC]
    public void Skill_WatcherFreeze_Start(float cooldown)
    {
        WatcherAudioManager.Instance.PlayAudioClip("skill_FreezeEffectSound");

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<WatcherSmash>().enabled = false;
        transform.Find("Ice").gameObject.SetActive(true);

        StartCoroutine(FreezeDurationCoroutine(cooldown));
    }

    private IEnumerator FreezeDurationCoroutine(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        photonView.RPC("Skill_WatcherFreeze_End", RpcTarget.All);
    }

    [PunRPC]
    public void Skill_WatcherFreeze_End()
    {
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<WatcherSmash>().enabled = true;
        transform.Find("Ice").gameObject.SetActive(false);
    }

    [PunRPC]
    public void Skill_WatcherVisibilityBlock_Start(float cooldown)
    {
        WatcherAudioManager.Instance.PlayAudioClip("skill_CloseSightEffectSound");
        UIManager.Instance.pnlWatcherVisibilityBlocker.gameObject.SetActive(true);

        StartCoroutine(VisibilityBlockDurationCoroutine(cooldown));
    }

    private IEnumerator VisibilityBlockDurationCoroutine(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        photonView.RPC("Skill_WatcherVisibilityBlock_End", RpcTarget.All);
    }

    [PunRPC]
    public void Skill_WatcherVisibilityBlock_End()
    {
        UIManager.Instance.pnlWatcherVisibilityBlocker.gameObject.SetActive(false);
    }
}*/

using Photon.Pun;
using System.Collections;
using UnityEngine;

public class WatcherEffectedFromPlayerSkills : MonoBehaviourPun
{
    public static WatcherEffectedFromPlayerSkills Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object role))
        {
            if (role.ToString() == "Watchers" && photonView.IsMine)
                StartCoroutine(DelayedRegisterToHumans());
        }
    }

    private IEnumerator DelayedRegisterToHumans()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerSkills[] allSkills = FindObjectsOfType<PlayerSkills>();
        foreach (var skill in allSkills)
        {
            skill.photonView.RPC("RegisterWatcher", skill.photonView.Owner, photonView.ViewID);
        }
    }

    [PunRPC]
    public void Skill_WatcherFreeze_Start(float cooldown)
    {
        WatcherAudioManager.Instance.PlayAudioClip("skill_FreezeEffectSound");

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<WatcherSmash>().enabled = false;
        transform.Find("Ice").gameObject.SetActive(true);

        StartCoroutine(FreezeDurationCoroutine(cooldown));
    }

    private IEnumerator FreezeDurationCoroutine(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        photonView.RPC("Skill_WatcherFreeze_End", RpcTarget.All);
    }

    [PunRPC]
    public void Skill_WatcherFreeze_End()
    {
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<WatcherSmash>().enabled = true;
        transform.Find("Ice").gameObject.SetActive(false);
    }

    [PunRPC]
    public void Skill_WatcherVisibilityBlock_Start(float cooldown)
    {
        WatcherAudioManager.Instance.PlayAudioClip("skill_CloseSightEffectSound");
        UIManager.Instance.pnlWatcherVisibilityBlocker.gameObject.SetActive(true);

        StartCoroutine(VisibilityBlockDurationCoroutine(cooldown));
    }

    private IEnumerator VisibilityBlockDurationCoroutine(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        photonView.RPC("Skill_WatcherVisibilityBlock_End", RpcTarget.All);
    }

    [PunRPC]
    public void Skill_WatcherVisibilityBlock_End()
    {
        UIManager.Instance.pnlWatcherVisibilityBlocker.gameObject.SetActive(false);
    }
}

