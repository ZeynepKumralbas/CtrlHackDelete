using UnityEngine;
using Cinemachine;
using System.Collections;

public class CutsceneCameraController : MonoBehaviour
{
    [Header("Cinemachine Virtual Cameras")]
    [SerializeField] private CinemachineVirtualCamera cmMain;
    [SerializeField] private CinemachineVirtualCamera cmApproach;
    [SerializeField] private CinemachineVirtualCamera cmButton;
    [SerializeField] private CinemachineVirtualCamera cmWide;

    private bool hasSwitchedToApproach = false;
    private bool hasSwitchedToButton = false;
    private bool hasSwitchedToWide = false;

    private void Start()
    {
        SwitchCamera(cmMain); // Başlangıçta cmMain aktif
    }

    private void SwitchCamera(CinemachineVirtualCamera targetCam)
    {
        cmMain.Priority = 0;
        cmApproach.Priority = 0;
        cmButton.Priority = 0;
        cmWide.Priority = 0;

        targetCam.Priority = 10;
        Debug.Log("Switched to camera: " + targetCam.name);
    }

    public void OnLeadCharacterArrived()
    {
        if (!hasSwitchedToApproach)
        {
            hasSwitchedToApproach = true;
            SwitchCamera(cmApproach);
        }
    }

    public void OnAllCharactersReady()
    {
        if (!hasSwitchedToButton)
        {
            hasSwitchedToButton = true;
            SwitchCamera(cmButton);
            StartCoroutine(SwitchToWideAfterDelay(2f)); // 2sn sonra geniş açı
        }
    }

    private IEnumerator SwitchToWideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!hasSwitchedToWide)
        {
            hasSwitchedToWide = true;
            SwitchCamera(cmWide);
        }
    }
}
