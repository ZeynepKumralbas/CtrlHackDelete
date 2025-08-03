/*using Photon.Pun;
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
    [SerializeField] private InputActionReference[] skills;

    [SerializeField] private float[] cooldownsForSkills;

    private Color imgAlpha;

    public TextMeshProUGUI[] txtTimersForSkills;

    private PlayerInteraction playerInteraction;

    public float[] activeCooldownsForSkills;

    public PhotonView view;
    public PhotonView watcherView;


    private void Start()
    {
        if (!view.IsMine) return;

        playerInteraction = FindObjectOfType<PlayerInteraction>();

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
        if (GetComponent<PlayerStateManager>().currentState == PlayerState.Ghost) return; // 👻 Beceri engeli

        // Cooldown'lar� azalt ve UI'yi g�ncelle
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
        if(playerInteraction.finishedMissionCounter > 0)
        {
            switch (playerInteraction.finishedMissionCounter)
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
        if(!view.IsMine) return;

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
/*    private void Skill_ColorChanger()
    {
        ModularRobotRandomizer.Instance.RandomizeMaterialOffsets();
        WatcherEffectedFromPlayerSkills.Instance.SkillCooldown(2, cooldownsForSkills[2]);
    }

}
*/
/*
using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YourNamespaceHere;

public class PlayerSkills : MonoBehaviourPun
{
    [Header("Skill Inputs")]
    [SerializeField] private InputActionReference[] skills; // Skill inputları (tuşlar)

    [Header("Cooldown Settings")]
    [SerializeField] private float[] cooldownsForSkills;

    [Header("UI Elements")]
    public TextMeshProUGUI[] txtTimersForSkills;  // Skill cooldown timer UI'ları (ataması Start'ta yapılacak)

    private float[] activeCooldownsForSkills;     // Skill cooldown sayaçları

    private PlayerInteraction playerInteraction;  // Görev tamamlamaları için referans

    private Image[] skillButtonImages;            // Butonların görsellerinin alpha ayarı için

    private WatcherEffectedFromPlayerSkills watcherScript;

    private void Start()
    {
        if (!photonView.IsMine) return;

        playerInteraction = GetComponent<PlayerInteraction>();
        if (playerInteraction == null)
            Debug.LogWarning("PlayerInteraction bulunamadı!");

        // UIManager'dan skill butonlarını al ve txtTimersForSkills dizisini doldur
        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager instance bulunamadı!");
            return;
        }

        skillButtonImages = new Image[UIManager.Instance.humanSkills.Length];
        txtTimersForSkills = new TextMeshProUGUI[UIManager.Instance.humanSkills.Length];

        for (int i = 0; i < UIManager.Instance.humanSkills.Length; i++)
        {
            var skillGO = UIManager.Instance.humanSkills[i];
            skillButtonImages[i] = skillGO.GetComponent<Image>();
            var txtTimerTransform = skillGO.transform.Find("TxtTimer");
            if (txtTimerTransform != null)
                txtTimersForSkills[i] = txtTimerTransform.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogWarning($"TxtTimer bulunamadı: humanSkills[{i}]");
        }

        foreach (var txtTimer in txtTimersForSkills)
        {
            if (txtTimer != null)
                txtTimer.enabled = false;
        }

        // Cooldownlar sıfırsa default değer ata
        for (int i = 0; i < cooldownsForSkills.Length; i++)
        {
            if (cooldownsForSkills[i] == 0f)
                cooldownsForSkills[i] = 5f * (i + 1);
        }

        activeCooldownsForSkills = new float[cooldownsForSkills.Length];
        for (int i = 0; i < activeCooldownsForSkills.Length; i++)
        {
            activeCooldownsForSkills[i] = 0f;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (skills[0].action.WasPressedThisFrame())
        {
            Debug.Log("Skill 0'a basıldı.");
            if (watcherScript == null)
            {
                Debug.LogError("HATA: watcherScript NULL!");
            }
            else
            {
                Debug.Log("WatcherScript var, skill RPC gönderiliyor.");
            }
        }

        if (skills[1].action.WasPressedThisFrame())
            Debug.Log("Skill 1'e basıldı!");

        if (skills[2].action.WasPressedThisFrame())
            Debug.Log("Skill 2'ye basıldı!");

        if (GetComponent<PlayerStateManager>().currentState == PlayerState.Ghost) return; // Hayaletken skill kullanma

        UpdateCooldowns();

        if (playerInteraction.finishedMissionCounter > 0)
        {
            int missionCount = playerInteraction.finishedMissionCounter;

            if (missionCount == 1)
                SkillActivationController(1);
            else if (missionCount == 2)
                SkillActivationController(2);
            else if (missionCount >= 3)
                SkillActivationController(3);
            Debug.Log("Görev sayısı: " + playerInteraction.finishedMissionCounter);

        }
    }

    [PunRPC]
    public void RegisterWatcher(int watcherViewID)
    {
        PhotonView watcherPV = PhotonView.Find(watcherViewID);
        if (watcherPV != null)
        {
            watcherScript = watcherPV.GetComponent<WatcherEffectedFromPlayerSkills>();
            Debug.Log(watcherScript != null ? "Watcher kayıt edildi." : "Watcher component bulunamadı!");
        }
        else
        {
            Debug.LogError("Watcher PhotonView bulunamadı!");
        }
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < activeCooldownsForSkills.Length; i++)
        {
            if (activeCooldownsForSkills[i] > 0)
            {
                activeCooldownsForSkills[i] -= Time.deltaTime;

                TimeSpan t = TimeSpan.FromSeconds(activeCooldownsForSkills[i]);
                if (txtTimersForSkills[i] != null)
                    txtTimersForSkills[i].text = string.Format("{0:D2}.{1:D1}", t.Seconds, t.Milliseconds / 100);

                if (activeCooldownsForSkills[i] <= 0)
                {
                    activeCooldownsForSkills[i] = 0f;
                    if (txtTimersForSkills[i] != null)
                        txtTimersForSkills[i].enabled = false;
                    if (skillButtonImages[i] != null)
                    {
                        var c = skillButtonImages[i].color;
                        c.a = 1f; // Geri opak yap
                        skillButtonImages[i].color = c;
                    }
                }
            }
        }
    }

    private void SkillActivationController(int unlockedSkillCount)
    {
        if (!photonView.IsMine) return;

        // Skill 1
        if (unlockedSkillCount >= 1)
        {
            SkillImageAlpha(0);
            if (skills[0].action.WasPressedThisFrame() && activeCooldownsForSkills[0] <= 0f)
            {
                if (WatcherEffectedFromPlayerSkills.Instance != null)
            {
                WatcherEffectedFromPlayerSkills.Instance.photonView.RPC("Skill_WatcherFreeze_Start", RpcTarget.All, cooldownsForSkills[0]);
            }
            else
            {
                Debug.LogError("WatcherEffectedFromPlayerSkills.Instance null, skill gönderilemiyor!");
            }

                StartCooldown(0, cooldownsForSkills[0]);
            }
        }

        // Skill 2
        if (unlockedSkillCount >= 2)
        {
            SkillImageAlpha(1);
            if (skills[1].action.WasPressedThisFrame() && activeCooldownsForSkills[1] <= 0f)
            {
                WatcherEffectedFromPlayerSkills.Instance.photonView.RPC("Skill_WatcherVisibilityBlock_Start", RpcTarget.All, cooldownsForSkills[1]);
                StartCooldown(1, cooldownsForSkills[1]);
            }
        }

        // Skill 3
        if (unlockedSkillCount >= 3)
        {
            SkillImageAlpha(2);
            if (skills[2].action.WasPressedThisFrame() && activeCooldownsForSkills[2] <= 0f)
            {
                Skill_ColorChanger();
            }
        }
    }

    private void SkillImageAlpha(int index)
    {
        if (skillButtonImages[index] != null)
        {
            Color c = skillButtonImages[index].color;
            c.a = 1f;
            skillButtonImages[index].color = c;
        }
    }

    private void Skill_ColorChanger()
    {
        Debug.Log("Skill 3: Renk değiştirme çağrıldı.");
        if (ModularRobotRandomizer.Instance != null)
        {
            ModularRobotRandomizer.Instance.RandomizeMaterialOffsets();
            Debug.Log("Material değiştirildi.");
        }
        else
        {
            Debug.LogError("ModularRobotRandomizer.Instance NULL!");
        }

        StartCooldown(2, cooldownsForSkills[2]);
    }


    public void StartCooldown(int skillIndex, float cooldown)
    {
        if (skillIndex >= 0 && skillIndex < activeCooldownsForSkills.Length)
        {
            if (txtTimersForSkills[skillIndex] != null)
                txtTimersForSkills[skillIndex].enabled = true;

            activeCooldownsForSkills[skillIndex] = cooldown;

            if (skillButtonImages[skillIndex] != null)
            {
                Color c = skillButtonImages[skillIndex].color;
                c.a = 0.5f; // Soğuma süresince butonu biraz şeffaf yap
                skillButtonImages[skillIndex].color = c;
            }
        }
    }
}*/

using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YourNamespaceHere;

public class PlayerSkills : MonoBehaviourPun
{
    [Header("Skill Inputs")]
    [SerializeField] private InputActionReference[] skills;

    [Header("Cooldown Settings")]
    [SerializeField] private float[] cooldownsForSkills;

    [Header("UI Elements")]
    public TextMeshProUGUI[] txtTimersForSkills;

    private float[] activeCooldownsForSkills;
    private Image[] skillButtonImages;

    private PlayerInteraction playerInteraction;
    private int previousSkillLevel = 0;
    private ModularRobotRandomizer myRobotRandomizer;

    private void Start()
    {
        if (!photonView.IsMine) return;

        playerInteraction = GetComponent<PlayerInteraction>();

        skillButtonImages = new Image[UIManager.Instance.humanSkills.Length];
        txtTimersForSkills = new TextMeshProUGUI[UIManager.Instance.humanSkills.Length];

        for (int i = 0; i < UIManager.Instance.humanSkills.Length; i++)
        {
            var skillGO = UIManager.Instance.humanSkills[i];
            skillButtonImages[i] = skillGO.GetComponent<Image>();
            var txtTimerTransform = skillGO.transform.Find("TxtTimer");
            txtTimersForSkills[i] = txtTimerTransform?.GetComponent<TextMeshProUGUI>();
        }

        for (int i = 0; i < txtTimersForSkills.Length; i++)
            if (txtTimersForSkills[i] != null)
                txtTimersForSkills[i].enabled = false;

        for (int i = 0; i < cooldownsForSkills.Length; i++)
            if (cooldownsForSkills[i] == 0f)
                cooldownsForSkills[i] = 5f * (i + 1);

        activeCooldownsForSkills = new float[cooldownsForSkills.Length];

        myRobotRandomizer = GetComponent<ModularRobotRandomizer>();

    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (GetComponent<PlayerStateManager>().currentState == PlayerState.Ghost) return;

        UpdateCooldowns();

        int currentSkillLevel = Mathf.Clamp(playerInteraction.finishedMissionCounter, 0, 3);
        if (currentSkillLevel != previousSkillLevel)
        {
            ActivateSkillVisuals(currentSkillLevel);
            previousSkillLevel = currentSkillLevel;
        }

        HandleSkillInputs(currentSkillLevel);
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < activeCooldownsForSkills.Length; i++)
        {
            if (activeCooldownsForSkills[i] > 0)
            {
                activeCooldownsForSkills[i] -= Time.deltaTime;

                if (txtTimersForSkills[i] != null)
                {
                    TimeSpan t = TimeSpan.FromSeconds(activeCooldownsForSkills[i]);
                    txtTimersForSkills[i].text = $"{t.Seconds:D2}.{t.Milliseconds / 100:D1}";
                }

                if (activeCooldownsForSkills[i] <= 0)
                {
                    activeCooldownsForSkills[i] = 0f;
                    if (txtTimersForSkills[i] != null) txtTimersForSkills[i].enabled = false;
                    if (skillButtonImages[i] != null)
                    {
                        Color c = skillButtonImages[i].color;
                        c.a = 1f;
                        skillButtonImages[i].color = c;
                    }
                }
            }
        }
    }

    private void ActivateSkillVisuals(int skillCount)
    {
        for (int i = 0; i < skillCount; i++)
        {
            if (skillButtonImages[i] != null)
            {
                Color c = skillButtonImages[i].color;
                c.a = 1f;
                skillButtonImages[i].color = c;
            }
        }
    }

    private void HandleSkillInputs(int skillLevel)
    {
        for (int i = 0; i < skillLevel; i++)
        {
            if (skills[i].action.WasPressedThisFrame() && activeCooldownsForSkills[i] <= 0f)
            {
                switch (i)
                {
                    case 0:
                        WatcherEffectedFromPlayerSkills.Instance?.photonView.RPC("Skill_WatcherFreeze_Start", RpcTarget.All, cooldownsForSkills[0]);
                        break;
                    case 1:
                        WatcherEffectedFromPlayerSkills.Instance?.photonView.RPC("Skill_WatcherVisibilityBlock_Start", RpcTarget.All, cooldownsForSkills[1]);
                        break;
                    case 2:
                        Debug.Log("Change color");
                        UseColorChangeSkill();
                        break;
                }
                StartCooldown(i, cooldownsForSkills[i]);
            }
        }
    }

private void UseColorChangeSkill()
{
    if (!photonView.IsMine) return;

    if (myRobotRandomizer != null)
    {
        // Burada myRobotRandomizer'ın photonView'u üzerinden RPC çağır
        myRobotRandomizer.photonView.RPC("RandomizeMaterialOffsets", RpcTarget.All);
    }
    else
    {
        Debug.LogError("myRobotRandomizer bulunamadı!");
    }
}


    public void StartCooldown(int skillIndex, float cooldown)
    {
        if (txtTimersForSkills[skillIndex] != null)
            txtTimersForSkills[skillIndex].enabled = true;

        activeCooldownsForSkills[skillIndex] = cooldown;

        if (skillButtonImages[skillIndex] != null)
        {
            Color c = skillButtonImages[skillIndex].color;
            c.a = 0.5f;
            skillButtonImages[skillIndex].color = c;
        }
    }

    [PunRPC]
    public void RegisterWatcher(int watcherViewID)
    {
        var watcherView = PhotonView.Find(watcherViewID);
        if (watcherView != null)
        {
            Debug.Log("Watcher registered.");
        }
    }
}

