using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using YourNamespaceHere;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Instance;

    [SerializeField] private InputActionReference[] skills;

    [SerializeField] private float[] cooldownsForSkills;

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
                txtTimersForSkills[i].text = string.Format("{0:D2}.{1:D2}", t.Seconds, t.Milliseconds / 100);

                if (activeCooldownsForSkills[i] <= 0)
                {
                    activeCooldownsForSkills[i] = 0;
                    txtTimersForSkills[i].enabled = false;
                }
            }
        }

        if (skills[0].action.WasPressedThisFrame() && activeCooldownsForSkills[0] <= 0f) // Watcher 5 sn dondurma
        {
            WatcherEffectedFromPlayerSkills.Instance.view.RPC("Skill_WatcherFreeze_Start", RpcTarget.All, cooldownsForSkills[0]);
        }
        else if (skills[1].action.WasPressedThisFrame() && activeCooldownsForSkills[1] <= 0f) // Watcher 3 sn gorus kisitlama
        {
            WatcherEffectedFromPlayerSkills.Instance.view.RPC("Skill_WatcherVisibilityBlock_Start", RpcTarget.All, cooldownsForSkills[1]);
        }
        else if (skills[2].action.WasPressedThisFrame() && activeCooldownsForSkills[2] <= 0f) // Human renk degistirme
        {
            Skill_ColorChanger();
        }

    }
    
    /*************************************************************/
    private void Skill_ColorChanger()
    {
        ModularRobotRandomizer.Instance.RandomizeMaterialOffsets();
        WatcherEffectedFromPlayerSkills.Instance.SkillCooldown(2, cooldownsForSkills[2]);
    }

}
