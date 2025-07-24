using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WatcherInteraction : MonoBehaviour
{
    public static WatcherInteraction Instance;

    private Collider hitboxCollider;

    public bool inHitbox = false;
    public bool isPlayer = false;

    public PhotonView targetView;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hitboxCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inHitbox = true;
            targetView = other.gameObject.GetComponent<PhotonView>();
            isPlayer = true;
        }
        else if (other.gameObject.CompareTag("NPC"))
        {
            inHitbox = true;
            targetView = other.gameObject.GetComponent<PhotonView>();
            isPlayer = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("NPC"))
        {
            inHitbox = false;
            targetView = null;
            isPlayer = false;
        }
    }
}
