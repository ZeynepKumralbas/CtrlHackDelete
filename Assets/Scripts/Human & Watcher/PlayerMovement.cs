/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 _moveDirection;
    private Rigidbody rb;

    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference run;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    private Animator _animator;

    // Multiplayer
    public PhotonView view;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!view.IsMine)
        {
            return;
        }

        _moveDirection = move.action.ReadValue<Vector2>();

        // Animasyona h�z bilgisi g�nder (0 = idle, 0.5 = y�r�me, 1 = ko�u)
        float inputMagnitude = _moveDirection.magnitude;
        bool isRunning = run.action.IsPressed();
        float animationSpeed = inputMagnitude * (isRunning ? 1f : 0.5f);

        _animator.SetFloat("speed", animationSpeed);
    }

    void FixedUpdate()
    {
        if (!view.IsMine)
        {
            return;
        }

        bool isRunning = run.action.IsPressed();
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 movement = new Vector3(_moveDirection.x, 0, _moveDirection.y) * currentSpeed;
        rb.velocity = movement;

        // Hareket varsa y�n�n� de�i�tir
        if (_moveDirection != Vector2.zero)
        {
            Vector3 lookDirection = new Vector3(_moveDirection.x, 0f, _moveDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    public Vector2 _moveDirection;
    private Rigidbody rb;

    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference run;

    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    private Animator _animator;

    // Multiplayer
    public PhotonView view;

    // Remote oyuncu için network senkronizasyon verileri
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkVelocity;
    private float networkAnimSpeed;
    private double networkTimestamp;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        view = GetComponent<PhotonView>();

        // Remote başlangıç pozisyonu local konuma eşitlenir
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    void Update()
    {
        if (view.IsMine)
        {
            _moveDirection = move.action.ReadValue<Vector2>();

            float inputMagnitude = _moveDirection.magnitude;
            bool isRunning = run.action.IsPressed();
            float animationSpeed = inputMagnitude * (isRunning ? 1f : 0.5f);

            _animator.SetFloat("speed", animationSpeed);
        }
        else
        {
            // Remote oyuncular için pozisyon ve animasyon interpolasyonu

            // FPS farkını dengelemek için interpolation factor
            float lerpFactor = Mathf.Clamp01(15f * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, networkPosition, lerpFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, lerpFactor);

            // Animasyon parametresi de yumuşak geçsin
            float currentSpeed = _animator.GetFloat("speed");
            _animator.SetFloat("speed", Mathf.Lerp(currentSpeed, networkAnimSpeed, lerpFactor));
        }
    }

    void FixedUpdate()
    {
        if (!view.IsMine)
            return;

        bool isRunning = run.action.IsPressed();
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 movement = new Vector3(_moveDirection.x, 0, _moveDirection.y) * currentSpeed;
        rb.velocity = movement;

        if (_moveDirection != Vector2.zero)
        {
            Vector3 lookDirection = new Vector3(_moveDirection.x, 0f, _moveDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(_animator.GetFloat("speed"));
            stream.SendNext(PhotonNetwork.Time);
        }
        else
        {
            Vector3 receivedPosition = (Vector3)stream.ReceiveNext();
            Quaternion receivedRotation = (Quaternion)stream.ReceiveNext();
            Vector3 receivedVelocity = (Vector3)stream.ReceiveNext();
            float receivedAnimSpeed = (float)stream.ReceiveNext();
            double receivedTime = (double)stream.ReceiveNext();

            double lag = PhotonNetwork.Time - receivedTime;

            // Lag compensation
            networkPosition = receivedPosition + receivedVelocity * (float)lag;
            networkRotation = receivedRotation;
            networkVelocity = receivedVelocity;
            networkAnimSpeed = receivedAnimSpeed;
            networkTimestamp = receivedTime;
        }
    }
}
