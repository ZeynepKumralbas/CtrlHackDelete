using UnityEngine;
using Photon.Pun;
using StarterAssets;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviourPun
{
    //   public ThirdPersonController thirdPersonController;
    //   public PlayerInput playerInput;

    public PhotonView view;

    void Start()
    {
        if (!view.IsMine)
        {
            // Diğer oyuncuların kameralarını ve kontrollerini kapat
            Transform cameraTransform = transform.Find("PlayerCameraRoot/MainCamera"); // Kamera yolunu kendine göre düzelt
            if (cameraTransform != null)
                Destroy(cameraTransform.gameObject);

            Destroy(GetComponent<FirstPersonController>());
            Destroy(GetComponent<ThirdPersonController>());
            Destroy(GetComponent<PlayerInput>());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    
}
