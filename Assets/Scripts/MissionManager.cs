using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    private TMP_Dropdown drnMissionList;

    private GameObject selectedMission;
    private CinemachineVirtualCamera playerCam;

    [SerializeField] private List<string> missionList = new List<string>();

    public int missionCount;

    public PhotonView view;
    void Start()
    {
        Instance = this;

        missionCount = transform.childCount;

        drnMissionList = UIManager.Instance.missionListDropdown;

        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                missionList.Add(transform.GetChild(i).gameObject.name);
            }
        }

        drnMissionList.AddOptions(missionList);

        drnMissionList.onValueChanged.AddListener(OnMissionSelected);

        if(view.IsMine) 
            playerCam = FindObjectOfType<CinemachineVirtualCamera>();
    }
    void Update()
    {
        if (!view.IsMine || selectedMission == null) return;


        /* OYUN SONU SENARYOSU ---> GOREVLER BITIRILIRSE*/
        if (missionCount == 0)
        {
            if (!GameEndManager.Instance.gameEnded)
            {
                GameEndManager.Instance.photonView.RPC("RPC_EndGame", RpcTarget.All, "HumansWin");
            }
        }

    }

    public void RemoveMissionAndRedirect(string missionName)
    {
        int index = missionList.IndexOf(missionName);

        if (index >= 0)
        {
            missionList.RemoveAt(index);
            drnMissionList.options.RemoveAt(index);
            Debug.Log("deneme");
            // E�er mevcut se�im silinen g�revse, �nce ge�ici olarak s�f�ra ayarla
            if (drnMissionList.value == index)
                drnMissionList.value = 0;

            drnMissionList.RefreshShownValue();
        }

        if (missionList.Count > 0)
        {
            int randomIndex = Random.Range(0, missionList.Count);
            drnMissionList.value = randomIndex; // Bu, OnMissionSelected tetikler
        }
        else
        {
            selectedMission = null;
        }
    }

    private void OnMissionSelected(int index)
    {
        selectedMission = gameObject.transform.Find(drnMissionList.options[index].text).gameObject;
        Debug.Log(selectedMission.name);
    }

}
