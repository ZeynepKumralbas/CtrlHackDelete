using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviourPun
{
    [SerializeField] private InputActionReference interaction;
    [SerializeField] private float interactionTime = 5f;
    [SerializeField] private GameObject txtInteractionButton;
    [SerializeField] private Slider missionCompletePercentSlider;

    private Animator _animator;

    private float defaultInteractionTime;

    private bool isInMissionPoint = false;
    private bool isHolding = false;

    private Coroutine holdCoroutine;

    private GameObject currentMissionPoint;

    // Multiplayer
    public PhotonView view;

    void Start()
    {
        _animator = GetComponent<Animator>();
        defaultInteractionTime = interactionTime;

        if (view.IsMine)
        {
            txtInteractionButton = UIManager.Instance.txtInteractionButton;
            missionCompletePercentSlider = UIManager.Instance.missionCompletePercentSlider;

            if (txtInteractionButton != null)
                txtInteractionButton.SetActive(false);

            if (missionCompletePercentSlider != null)
            {
                missionCompletePercentSlider.gameObject.SetActive(false);
                missionCompletePercentSlider.maxValue = interactionTime;
                missionCompletePercentSlider.value = interactionTime;
            }
        }
    }

    void Update()
    {
        if (!view.IsMine || !isInMissionPoint) return;

        if (interaction.action.WasPressedThisFrame())
        {
            holdCoroutine = StartCoroutine(HoldInteraction());
        }

        if (interaction.action.WasReleasedThisFrame())
        {
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
                view.RPC("SetInteractingAnim", RpcTarget.All, false);
                Debug.Log("Tuş erken bırakıldı, işlem iptal.");
            }
        }
    }

    private IEnumerator HoldInteraction()
    {
        isHolding = true;
        view.RPC("SetInteractingAnim", RpcTarget.All, true);
        float holdTime = 0f;

        if (missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(true);
            missionCompletePercentSlider.value = interactionTime;
        }

        while (holdTime < interactionTime)
        {
            if (!interaction.action.IsPressed())
            {
                view.RPC("SetInteractingAnim", RpcTarget.All, false);

                if (missionCompletePercentSlider != null)
                {
                    missionCompletePercentSlider.gameObject.SetActive(false);
                    missionCompletePercentSlider.value = interactionTime;
                }

                yield break;
            }

            holdTime += Time.deltaTime;

            if (missionCompletePercentSlider != null)
            {
                missionCompletePercentSlider.value = interactionTime - holdTime;
            }

            yield return null;
        }

        Debug.Log("Görev tamamlandı!");
        view.RPC("SetInteractingAnim", RpcTarget.All, false);
        isHolding = false;

        if (missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(false);
            missionCompletePercentSlider.value = 0;
        }

        if (currentMissionPoint != null)
        {
            currentMissionPoint.SetActive(false);
            interactionTime = defaultInteractionTime;

            if (txtInteractionButton != null)
                txtInteractionButton.SetActive(false);
        }
    }

    [PunRPC]
    private void SetInteractingAnim(bool isInteracting)
    {
        _animator.SetBool("isInteracting", isInteracting);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!view.IsMine) return;

        if (other.CompareTag("MissionPoint"))
        {
            isInMissionPoint = true;
            currentMissionPoint = other.gameObject;

            string difficulty = currentMissionPoint.transform.parent.tag;

            switch (difficulty)
            {
                case "EasyMission":
                    interactionTime = defaultInteractionTime * 1;
                    break;
                case "NormalMission":
                    interactionTime = defaultInteractionTime * 2;
                    break;
                case "HardMission":
                    interactionTime = defaultInteractionTime * 3;
                    break;
            }

            missionCompletePercentSlider.maxValue = interactionTime;

            if (txtInteractionButton != null)
            {
                txtInteractionButton.transform.position = currentMissionPoint.transform.position + new Vector3(0f, 1.5f, 0f);
                txtInteractionButton.SetActive(true);
            }

            Debug.Log("Görev alanına girildi");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!view.IsMine) return;

        if (other.CompareTag("MissionPoint"))
        {
            interactionTime = defaultInteractionTime;
            isInMissionPoint = false;
            currentMissionPoint = null;

            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
                view.RPC("SetInteractingAnim", RpcTarget.All, false);
            }

            if (missionCompletePercentSlider != null)
            {
                missionCompletePercentSlider.gameObject.SetActive(false);
                missionCompletePercentSlider.value = interactionTime;
            }

            if (txtInteractionButton != null)
                txtInteractionButton.SetActive(false);

            Debug.Log("Görev alanı terk edildi, işlem iptal.");
        }
    }
}
