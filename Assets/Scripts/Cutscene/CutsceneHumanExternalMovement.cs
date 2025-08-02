using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CutsceneHumanExternalMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference run;
    [SerializeField] private InputActionReference sneak;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float sneakSpeed = 1.5f; // Walk'tan da yavaş

    private Rigidbody rb;
    private Animator animator;
    private Vector2 inputDirection;

    private bool useExternalInput = false;
    private bool overrideRun = false;
    private bool overrideSneak = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        move.action.Enable();
        run.action.Enable();
        sneak.action.Enable();
    }

    private void Update()
    {
        if (!useExternalInput)
        {
            inputDirection = move.action.ReadValue<Vector2>();
            overrideRun = run.action.IsPressed();
            overrideSneak = sneak.action.IsPressed();

            float inputMagnitude = inputDirection.magnitude;
            float animSpeed = inputMagnitude * (overrideRun ? 1f : overrideSneak ? 0.3f : 0.5f);
            animator.SetFloat("speed", animSpeed);
            animator.SetBool("isSneaking", overrideSneak);
        }
    }



    private void FixedUpdate()
    {
        bool isRunning = run.action.IsPressed();
        bool isSneaking = sneak.action.IsPressed();

        float currentSpeed = overrideRun ? runSpeed : (overrideSneak ? sneakSpeed : walkSpeed);

        Vector3 moveVector = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;

        // 🚶 Hareket uygula
        Vector3 velocity = moveVector * currentSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // y eksenini koru

        // 🔁 Dönüş uygula
        if (moveVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetExternalInput(Vector2 direction, bool isRunning, bool isSneaking)
    {
        this.useExternalInput = true;
        this.overrideRun = isRunning;
        this.overrideSneak = isSneaking;

        inputDirection = direction;

        float inputMagnitude = direction.magnitude;

        animator.SetBool("isSneaking", isSneaking);

        float animSpeed = 0f;
        if (isRunning)
            animSpeed = inputMagnitude * 1f;
        else if (isSneaking)
            animSpeed = inputMagnitude * 0.1f; // 👈 Sneak için daha düşük
        else
            animSpeed = inputMagnitude * 0.5f;

        animator.SetFloat("speed", animSpeed);

        // 🧪 DEBUG:
        Debug.Log($"ExternalInput -> Dir: {direction}, Run: {isRunning}, Sneak: {isSneaking}, Speed: {animSpeed}");
    }

public void MoveTo(Vector3 worldTarget, bool isRunning, bool isSneaking, System.Action onArrived = null)
{
    StopAllCoroutines();
    StartCoroutine(MoveToRoutine(worldTarget, isRunning, isSneaking, onArrived));
}

private IEnumerator MoveToRoutine(Vector3 target, bool isRunning, bool isSneaking, System.Action onArrived)
{
    useExternalInput = true;
    overrideRun = isRunning;
    overrideSneak = isSneaking;

    float distanceThreshold = 0.1f;

    while (Vector3.Distance(transform.position, target) > distanceThreshold)
    {
        Vector3 dir = (target - transform.position).normalized;
        Vector2 input2D = new Vector2(dir.x, dir.z); // XZ düzlemi

        SetExternalInput(input2D, isRunning, isSneaking);
        yield return null;
    }

    // Durdur
    SetExternalInput(Vector2.zero, false, false);

    // Callback tetikle
    onArrived?.Invoke();
}





}