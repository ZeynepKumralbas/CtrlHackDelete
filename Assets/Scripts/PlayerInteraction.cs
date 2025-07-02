using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
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
        view = GetComponent<PhotonView>();
        defaultInteractionTime = interactionTime;
        _animator = GetComponent<Animator>();
        txtInteractionButton = UIManager.Instance.txtInteractionButton;
        missionCompletePercentSlider = UIManager.Instance.missionCompletePercentSlider;

        if(txtInteractionButton != null) 
            txtInteractionButton.SetActive(false);

        if (missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(false);
            missionCompletePercentSlider.maxValue = interactionTime;
            missionCompletePercentSlider.value = interactionTime;
        }

    }
    void Update()
    {
        if (!view.IsMine)
        {
            return;
        }

        if (!isInMissionPoint) return; //G�rev alan�nda de�ilse kod �al��maz

        if (interaction.action.WasPressedThisFrame()) //Etkile�im tu�una bas�ld��� anda Corotine'i ba�lat
        {
            holdCoroutine = StartCoroutine(HoldInteraction());
        }

        if (interaction.action.WasReleasedThisFrame()) //Etkile�im tu�una basma b�rak�ld��� anda Corotine'i ve animasyonu durdur
        {
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
                _animator.SetBool("isInteracting", false);
                Debug.Log("Tu� erken b�rak�ld�, i�lem iptal.");
            }
        }
    }

    private IEnumerator HoldInteraction() //Tu�a bas�ld��� s�re boyunca Coroutine ve animasyon �al���r
    {
        isHolding = true;
        _animator.SetBool("isInteracting", true);
        float holdTime = 0f;

        if(missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(true);
            missionCompletePercentSlider.value = interactionTime;
        }

        while (holdTime < interactionTime) //Tu�a ilgili s�re kadar bas�lmas� istenir
        {
            if (!interaction.action.IsPressed()) //Tu� b�rak�ld���nda animasyon durur ve Coroutine'den ��k�l�r
            {
                _animator.SetBool("isInteracting", false);

                if (missionCompletePercentSlider != null)
                {
                    missionCompletePercentSlider.gameObject.SetActive(true);
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

        //�lgili s�re kadar bas�l�rsa g�rev tamamlan�r, Coroutine ve animasyon durdurulur
        Debug.Log("G�rev tamamland�!");
        _animator.SetBool("isInteracting", false);
        isHolding = false;

        if (missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(false);
            missionCompletePercentSlider.value = 0;
        }

        if (currentMissionPoint != null) //G�rev alan� kapat�l�r
        {
            currentMissionPoint.SetActive(false);

            interactionTime = defaultInteractionTime;

            if (txtInteractionButton != null)
                txtInteractionButton.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other) //G�rev alan�na giri� kontrolc�s�
    {
        float defaultInteractionTime = interactionTime;
        if (other.CompareTag("MissionPoint"))
        {
            Debug.Log("onTrigger => missionCompletePercentSlider:" + missionCompletePercentSlider + " txtInteractionButton:" + txtInteractionButton);

            isInMissionPoint = true;
            currentMissionPoint = other.gameObject;

            switch (currentMissionPoint.transform.GetChild(0).tag)
            {
                case "EasyMission":
                    interactionTime = interactionTime* 1;
                    missionCompletePercentSlider.maxValue = interactionTime;
                    break;
                case "NormalMission":
                    interactionTime = interactionTime * 2;
                    missionCompletePercentSlider.maxValue = interactionTime;
                    break;
                case "HardMission":
                    interactionTime = interactionTime * 3;
                    missionCompletePercentSlider.maxValue = interactionTime;
                    break;
                default:
                    break;
            }

            if (txtInteractionButton != null)
            {
                txtInteractionButton.transform.position = currentMissionPoint.transform.position + new Vector3(0f, 1.5f, 0f);  
                txtInteractionButton.SetActive(true);
            }

            Debug.Log("G�rev alan�na girildi");
        }
    }

    private void OnTriggerExit(Collider other) //G�rev alan�ndan ��k�� kontrolc�s�
    {
        if (other.CompareTag("MissionPoint"))
        {
            interactionTime = defaultInteractionTime;

            isInMissionPoint = false;
            currentMissionPoint = null;

            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
                _animator.SetBool("isInteracting", false);
            }

            if (missionCompletePercentSlider != null)
            {
                missionCompletePercentSlider.gameObject.SetActive(false);
                missionCompletePercentSlider.value = interactionTime;
            }

            if (txtInteractionButton != null)
                txtInteractionButton.SetActive(false);

            Debug.Log("G�rev alan� terk edildi, i�lem iptal.");

        }
    }
}
