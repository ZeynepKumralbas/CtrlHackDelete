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

    public PhotonView view;

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

            // 🔊 YÜRÜME / KOŞMA SESLERİ — sadece kendi karakterin için tetiklenir ama ses tüm client'lara gider
            if (inputMagnitude > 0.1f)
            {
                if (isRunning)
                {
                    PlayerAudioManager.Instance?.PlayAudioClip("runningSound");
                    WatcherAudioManager.Instance?.PlayAudioClip("runningSound");

                }
                else
                {
                    PlayerAudioManager.Instance?.PlayAudioClip("walkingSound");
                    WatcherAudioManager.Instance?.PlayAudioClip("walkingSound");
                }
            }
            else
            {
                PlayerAudioManager.Instance?.StopLoopingAudio();
                WatcherAudioManager.Instance?.StopLoopingAudio();
            }
        }
        else
        {
            float lerpFactor = Mathf.Clamp01(15f * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, networkPosition, lerpFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, lerpFactor);

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

            networkPosition = receivedPosition + receivedVelocity * (float)lag;
            networkRotation = receivedRotation;
            networkVelocity = receivedVelocity;
            networkAnimSpeed = receivedAnimSpeed;
            networkTimestamp = receivedTime;
        }
    }
}
