using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CutsceneHumanMovement : MonoBehaviour
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
    inputDirection = move.action.ReadValue<Vector2>();

    float inputMagnitude = Mathf.Clamp01(inputDirection.magnitude);
    bool isRunning = run.action.IsPressed();
    bool isSneaking = sneak.action.IsPressed();

    float animSpeed = inputMagnitude * (
        isRunning ? 1f :
        isSneaking ? 0.3f : // Sneak yavaş hareket
        0.5f // Normal yürüyüş
    );

    animator.SetFloat("speed", animSpeed);
    animator.SetBool("isSneaking", isSneaking);
}


    private void FixedUpdate()
{
    bool isRunning = run.action.IsPressed();
    bool isSneaking = sneak.action.IsPressed();

    float currentSpeed = isRunning ? runSpeed : (isSneaking ? sneakSpeed : walkSpeed);

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

}