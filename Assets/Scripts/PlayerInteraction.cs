using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference interaction;
    [SerializeField] private float interactionTime = 10f;

    private Animator _animator;
    private bool isInMissionPoint = false;
    private bool isHolding = false;
    private Coroutine holdCoroutine;
    private GameObject currentMissionPoint;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (!isInMissionPoint) return;

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
                _animator.SetBool("isInteracting", false); // animasyonu durdur
                Debug.Log("Tuþ erken býrakýldý, iþlem iptal.");
            }
        }
    }

    private IEnumerator HoldInteraction()
    {
        isHolding = true;
        _animator.SetBool("isInteracting", true); // animasyonu baþlat
        float holdTime = 0f;

        while (holdTime < interactionTime)
        {
            if (!interaction.action.IsPressed())
            {
                _animator.SetBool("isInteracting", false); // animasyonu durdur
                yield break; // tuþ býrakýldýysa çýk
            }

            holdTime += Time.deltaTime;
            yield return null;
        }

        // Görev tamamlandý
        Debug.Log("Görev tamamlandý!");
        _animator.SetBool("isInteracting", false);
        isHolding = false;

        if (currentMissionPoint != null)
        {
            currentMissionPoint.SetActive(false); // görev alanýný kapat
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MissionPoint"))
        {
            isInMissionPoint = true;
            currentMissionPoint = other.gameObject;
            Debug.Log("Görev alanýna girildi");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MissionPoint"))
        {
            isInMissionPoint = false;
            currentMissionPoint = null;

            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
                _animator.SetBool("isInteracting", false);
                Debug.Log("Görev alaný terk edildi, iþlem iptal.");
            }
        }
    }
}
