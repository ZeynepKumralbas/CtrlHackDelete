using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraCutsceneManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcMainScreen;
    [SerializeField] private CinemachineVirtualCamera vcFollowCharacters;
    [SerializeField] private CinemachineVirtualCamera vcButtonPress;
    [SerializeField] private CinemachineVirtualCamera vcWideView;

    private void Start()
    {
        SetCamera(vcMainScreen); // başlangıç kamerası
    }

    public void PlayCameraSequence()
    {
        StartCoroutine(CameraRoutine());
    }

    private IEnumerator CameraRoutine()
    {
        yield return new WaitForSeconds(2f); // karakterler yaklaşırken

        SetCamera(vcFollowCharacters);

        yield return new WaitForSeconds(3f); // karakterler gelmeye devam ediyor

        SetCamera(vcButtonPress);

        yield return new WaitForSeconds(2f); // tuşa basma anı

        SetCamera(vcWideView); // Hack ekranları patlıyor
    }

    private void SetCamera(CinemachineVirtualCamera targetCam)
    {
        vcMainScreen.Priority = 0;
        vcFollowCharacters.Priority = 0;
        vcButtonPress.Priority = 0;
        vcWideView.Priority = 0;

        targetCam.Priority = 10;
    }
}
