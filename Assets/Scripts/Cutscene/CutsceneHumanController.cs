using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CutsceneHumanController : MonoBehaviour
{
    Animator animator;
    CharacterController controller;

    public float walkSpeed = 2f;
    public float runSpeed = 6f;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Hareket girişi
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical);
        float speed = move.magnitude;

        // Shift basılıysa koşuyoruz
        if (speed > 0.1f && Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        else if (speed > 0.1f)
        {
            speed = walkSpeed;
        }

        // Animator parametreleri güncelle
        animator.SetFloat("speed", speed);

        // Sneak (çömelme) C tuşu
        bool isSneaking = Input.GetKey(KeyCode.C);
        animator.SetBool("isSneaking", isSneaking);
    }
}
