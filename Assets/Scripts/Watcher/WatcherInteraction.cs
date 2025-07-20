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
    public PhotonView targetView;

    void Start()
    {
        Instance = this;
        hitboxCollider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Player"))
        {
            inHitbox = true;
            targetView = other.gameObject.GetComponent<PhotonView>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Player"))
        {
            inHitbox = false;
        }
    }
}
