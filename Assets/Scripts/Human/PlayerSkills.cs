using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YourNamespaceHere;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance;

    [SerializeField] private InputActionReference[] skills;

    [SerializeField] private float[] cooldownsForSkills;

    private Color imgAlpha;

    public TextMeshProUGUI[] txtTimersForSkills;


    public float[] activeCooldownsForSkills;

    public PhotonView view;
    public PhotonView watcherView;


    private void Start()
    {
        Instance = this;

        txtTimersForSkills = new TextMeshProUGUI[skills.Length];

        for(int i = 0; i < UIManager.Instance.humanSkills.Length; i++)
        {
            txtTimersForSkills[i] = UIManager.Instance.humanSkills[i].gameObject.transform.Find("TxtTimer").GetComponent<TextMeshProUGUI>();
        }
        foreach (var txtTimer in txtTimersForSkills)
        {
            txtTimer.enabled = false;
        }
        for(int i = 0; i < cooldownsForSkills.Length; i++)
        {
            if (cooldownsForSkills[i] == 0)
                cooldownsForSkills[i] = 5f * (i+1);
        }

        activeCooldownsForSkills = new float[cooldownsForSkills.Length];

        for (int i = 0; i < activeCooldownsForSkills.Length; i++)
        {
            activeCooldownsForSkills[i] = 0f;
        }

    }
    private void Update()
    {
        if (!view.IsMine) return;

        // Cooldown'larý azalt ve UI'yi güncelle
        for (int i = 0; i < activeCooldownsForSkills.Length; i++)
        {
            if (activeCooldownsForSkills[i] > 0)
            {
                activeCooldownsForSkills[i] -= Time.deltaTime;

                TimeSpan t = TimeSpan.FromSeconds(activeCooldownsForSkills[i]);
                txtTimersForSkills[i].text = string.Format("{0:D2}.{1:D1}", t.Seconds, t.Milliseconds / 100);

                if (activeCooldownsForSkills[i] <= 0)
                {
                    activeCooldownsForSkills[i] = 0;
                    txtTimersForSkills[i].enabled = false;
                }
            }
        }
        if(PlayerInteraction.Instance.finishedMissionCounter > 0)
        {
            switch (PlayerInteraction.Instance.finishedMissionCounter)
            {
                case 1:
                    SkillActivationController(1);
                    break;
                case 2:
                    SkillActivationController(2);
                    break;
                case int n when n >= 3:
                    SkillActivationController(3);
                    break;
                default:
                    Debug.Log("Yetenekleri acmak icin yapilan gorev sayisi yetersiz");
                    break;
            }
        }
    }
    private void SkillActivationController(int index)
    {
        if(index == 1)
        {
            SkillImageAlpha(0);
            if (skills[0].action.WasPressedThisFrame() && activeCooldownsForSkills[0] <= 0f) // Watcher 5 sn dondurma
            {
                WatcherEffectedFromPlayerSkills.Instance.view.RPC("Skill_WatcherFreeze_Start", RpcTarget.All, cooldownsForSkills[0]);
            }
        }else if(index == 2)
        {
            if (skills[0].action.WasPressedThisFrame() && activeCooldownsForSkills[0] <= 0f) // Watcher 5 sn dondurma
            {
                WatcherEffectedFromPlayerSkills.Instance.view.RPC("Skill_WatcherFreeze_Start", RpcTarget.All, cooldownsForSkills[0]);
            }
            SkillImageAlpha(1);
            if (skills[1].action.WasPressedThisFrame() && activeCooldownsForSkills[1] <= 0f) // Watcher 3 sn gorus kisitlama
            {
                WatcherEffectedFromPlayerSkills.Instance.view.RPC("Skill_WatcherVisibilityBlock_Start", RpcTarget.All, cooldownsForSkills[1]);
            }
        }else if(index == 3)
        {
            if (skills[0].action.WasPressedThisFrame() && activeCooldownsForSkills[0] <= 0f) // Watcher 5 sn dondurma
            {
                WatcherEffectedFromPlayerSkills.Instance.view.RPC("Skill_WatcherFreeze_Start", RpcTarget.All, cooldownsForSkills[0]);
            }
            if (skills[1].action.WasPressedThisFrame() && activeCooldownsForSkills[1] <= 0f) // Watcher 3 sn gorus kisitlama
            {
                WatcherEffectedFromPlayerSkills.Instance.view.RPC("Skill_WatcherVisibilityBlock_Start", RpcTarget.All, cooldownsForSkills[1]);
            }
            SkillImageAlpha(2);
            if (skills[2].action.WasPressedThisFrame() && activeCooldownsForSkills[2] <= 0f) // Human renk degistirme
            {
                Skill_ColorChanger();
            }
        }
    }
    private void SkillImageAlpha(int index)
    {
        imgAlpha = UIManager.Instance.humanSkills[index].GetComponent<Image>().color;
        imgAlpha.a = 1f;
        UIManager.Instance.humanSkills[index].GetComponent<Image>().color = imgAlpha;
    }
    /*************************************************************/
    private void Skill_ColorChanger()
    {
        ModularRobotRandomizer.Instance.RandomizeMaterialOffsets();
        WatcherEffectedFromPlayerSkills.Instance.SkillCooldown(2, cooldownsForSkills[2]);
    }

}
