using Photon.Pun;
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
    [PunRPC]
    public void Skill_WatcherFreeze_Start(float cooldown)
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<WatcherSmash>().enabled = false;
        transform.Find("Ice").gameObject.SetActive(true);

        SkillCooldown(0, cooldown);

        StartCoroutine(FreezeDurationCoroutine(cooldown));
    }
    private IEnumerator FreezeDurationCoroutine(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        view.RPC("Skill_WatcherFreeze_End", RpcTarget.All); // <-- HERKESE GÖNDER!
    }

    [PunRPC]
    public void Skill_WatcherFreeze_End()
    {
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<WatcherSmash>().enabled = true;
        transform.Find("Ice").gameObject.SetActive(false);

        _playerSkills.txtTimersForSkills[0].enabled = false;

        Debug.Log("Freeze sonlandý");
    }

    /***************************************************************/
    [PunRPC]
    public void Skill_WatcherVisibilityBlock_Start(float cooldown)
    {
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
        Debug.Log("Visibility block sonlandý");
    }

}
