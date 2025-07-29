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
    private TMP_Dropdown drnMissionList;
    private Image missionPointer;

    private GameObject selectedMission;
    private CinemachineVirtualCamera playerCam;

    [SerializeField] private List<string> missionList = new List<string>();

    public PhotonView view;
    void Start()
    {
        drnMissionList = UIManager.Instance.missionListDropdown;
        missionPointer = UIManager.Instance.imgMissionPointer;

        missionPointer.gameObject.SetActive(false);

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            //transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        for(int i = 0; i < transform.childCount; i++)
        {
            if (i > (transform.childCount / 2) + 1)
                break;

            int randomChoice = Random.Range(0, transform.childCount);
            if (!transform.GetChild(randomChoice).gameObject.activeSelf)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                //transform.GetChild(randomChoice).gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }

        }

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

        MissionPointerRotation();

    }
    private void MissionPointerRotation()
    {
        // UI pointer'�n ekran s�n�rlar� i�inde kalmas�n� sa�la
        float minX = missionPointer.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = missionPointer.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        // G�rev objesinin ekran pozisyonu
        Vector3 screenPos = Camera.main.WorldToScreenPoint(selectedMission.transform.position);

        // E�er hedef ekran�n arkas�ndaysa
        if (screenPos.z < 0)
        {
            // Ekran arkas�ndaysa i�areti ekran kenar�na sabitle
            screenPos *= -1;
            screenPos.x = (screenPos.x < Screen.width / 2) ? maxX : minX;
            screenPos.y = (screenPos.y < Screen.height / 2) ? maxY : minY;
        }

        // Ekran pozisyonunu s�n�rlara g�re k�rp
        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        // UI pointer'� konumland�r
        missionPointer.transform.position = screenPos;

        // Kamera ve hedefin d�nya pozisyonu aras�ndaki y�n vekt�r�
        Vector3 dir = selectedMission.transform.position - playerCam.transform.position;
        dir.y = 0f; // Sadece yatay y�n i�in

        // A��y� hesapla
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        // UI pointer'� d�nd�r
        missionPointer.transform.rotation = Quaternion.Euler(0, 0, angle);
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
            missionPointer.enabled = false;
        }
    }

    private void OnMissionSelected(int index)
    {
        missionPointer.gameObject.SetActive(true);

        selectedMission = gameObject.transform.Find(drnMissionList.options[index].text).gameObject;
        Debug.Log(selectedMission.name);
    }

}
