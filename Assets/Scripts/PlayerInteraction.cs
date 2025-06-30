using System.Collections;
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

    void Start()
    {
        defaultInteractionTime = interactionTime;

        _animator = GetComponent<Animator>();

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
        if (!isInMissionPoint) return; //Görev alanýnda deðilse kod çalýþmaz

        if (interaction.action.WasPressedThisFrame()) //Etkileþim tuþuna basýldýðý anda Corotine'i baþlat
        {
            holdCoroutine = StartCoroutine(HoldInteraction());
        }

        if (interaction.action.WasReleasedThisFrame()) //Etkileþim tuþuna basma býrakýldýðý anda Corotine'i ve animasyonu durdur
        {
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
                _animator.SetBool("isInteracting", false);
                Debug.Log("Tuþ erken býrakýldý, iþlem iptal.");
            }
        }
    }

    private IEnumerator HoldInteraction() //Tuþa basýldýðý süre boyunca Coroutine ve animasyon çalýþýr
    {
        isHolding = true;
        _animator.SetBool("isInteracting", true);
        float holdTime = 0f;

        if(missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(true);
            missionCompletePercentSlider.value = interactionTime;
        }

        while (holdTime < interactionTime) //Tuþa ilgili süre kadar basýlmasý istenir
        {
            if (!interaction.action.IsPressed()) //Tuþ býrakýldýðýnda animasyon durur ve Coroutine'den çýkýlýr
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

        //Ýlgili süre kadar basýlýrsa görev tamamlanýr, Coroutine ve animasyon durdurulur
        Debug.Log("Görev tamamlandý!");
        _animator.SetBool("isInteracting", false);
        isHolding = false;

        if (missionCompletePercentSlider != null)
        {
            missionCompletePercentSlider.gameObject.SetActive(false);
            missionCompletePercentSlider.value = 0;
        }

        if (currentMissionPoint != null) //Görev alaný kapatýlýr
        {
            currentMissionPoint.SetActive(false);

            interactionTime = defaultInteractionTime;

            if (txtInteractionButton != null)
                txtInteractionButton.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other) //Görev alanýna giriþ kontrolcüsü
    {
        float defaultInteractionTime = interactionTime;
        if (other.CompareTag("MissionPoint"))
        {
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

            Debug.Log("Görev alanýna girildi");
        }
    }

    private void OnTriggerExit(Collider other) //Görev alanýndan çýkýþ kontrolcüsü
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

            Debug.Log("Görev alaný terk edildi, iþlem iptal.");

        }
    }
}
