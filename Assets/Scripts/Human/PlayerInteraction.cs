using System.Collections;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviourPun
{
    public static PlayerInteraction Instance;

    private MissionManager missionManager;

    [SerializeField] private InputActionReference interaction;
    [SerializeField] private float interactionTime = 5f;
    [SerializeField] private GameObject txtInteractionButton;
    [SerializeField] private Slider missionCompletePercentSlider;

    private Animator _animator;

    private float defaultInteractionTime;

    public int finishedMissionCounter = 0;

    public string roomName;

    private bool isInMissionPoint = false;
    private bool isHolding = false;
    public bool isCompleted = false;

    private Coroutine holdCoroutine;

    private GameObject currentMissionPoint;
    private string currentMissionPointName;

    // Multiplayer
    public PhotonView view;

    void Start()
    {
        Instance = this;

        _animator = GetComponent<Animator>();
        missionManager = FindObjectOfType<MissionManager>();

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
        if (!view.IsMine) return;
        if (!isInMissionPoint) return;

        if (interaction.action.WasPressedThisFrame())
        {
            holdCoroutine = StartCoroutine(HoldInteraction());
        }

        if (interaction.action.WasReleasedThisFrame())
        {
            if (holdCoroutine != null)
            {
                isInMissionPoint = false;

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
        finishedMissionCounter++;
        isHolding = false;


        if (missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(false);
            missionCompletePercentSlider.value = 0;
        }

        if (currentMissionPoint != null)
        {
            currentMissionPointName = currentMissionPoint.transform.parent.name;

            PhotonView missionView = currentMissionPoint.GetComponentInParent<PhotonView>();
            if (missionView != null)
            {
                view.RPC("SetMissionVisibility", RpcTarget.All, missionView.ViewID, false);
            }
            view.RPC("SetMissionVisibility", RpcTarget.All, missionView.ViewID, false);

            interactionTime = defaultInteractionTime;

            if (txtInteractionButton != null)
                txtInteractionButton.SetActive(false);
        }

        if (missionManager != null && !string.IsNullOrEmpty(currentMissionPointName) && 
            !currentMissionPoint.activeSelf)
        {
            missionManager.RemoveMissionAndRedirect(currentMissionPointName);
        }


    }

    [PunRPC]
    private void SetInteractingAnim(bool isInteracting)
    {
        _animator.SetBool("isInteracting", isInteracting);
        isCompleted = !isInteracting;
        Debug.Log("isCompleted: " + isCompleted);
    }
    [PunRPC]
    private void SetMissionVisibility(int missionPhotonViewId, bool isVisible)
    {
        PhotonView missionView = PhotonView.Find(missionPhotonViewId);
        if (missionView != null)
        {
            Transform missionTransform = missionView.transform;
            Transform sphere = missionTransform.GetChild(0); // Child objesi

            if (sphere != null)
            {
                sphere.gameObject.SetActive(isVisible);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!view.IsMine) return;

        if (other.CompareTag("MissionPoint"))
        {
            isInMissionPoint = true;
            currentMissionPoint = other.gameObject;

            roomName = currentMissionPoint.transform.parent.gameObject.GetComponent<TaskPoint>().roomName;

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
