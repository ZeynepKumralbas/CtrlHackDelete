using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 _moveDirection;
    private Rigidbody rb;

    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference run;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    private Animator _animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();



        // Animasyona hýz bilgisi gönder (0 = idle, 0.5 = yürüme, 1 = koþu)
        float inputMagnitude = _moveDirection.magnitude;
        bool isRunning = run.action.IsPressed();
        float animationSpeed = inputMagnitude * (isRunning ? 1f : 0.5f);

        _animator.SetFloat("speed", animationSpeed);
    }

    void FixedUpdate()
    {
        bool isRunning = run.action.IsPressed();
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 movement = new Vector3(_moveDirection.x, 0, _moveDirection.y) * currentSpeed;
        rb.velocity = movement;

        // Hareket varsa yönünü deðiþtir
        if (_moveDirection != Vector2.zero)
        {
            Vector3 lookDirection = new Vector3(_moveDirection.x, 0f, _moveDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

    }
}
