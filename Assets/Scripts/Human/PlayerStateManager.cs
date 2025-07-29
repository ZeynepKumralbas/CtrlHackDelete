using UnityEngine;
using Photon.Pun;

public enum PlayerState
{
    Alive,
    Dead,
    Ghost
}

public class PlayerStateManager : MonoBehaviourPun
{
    public PlayerState currentState = PlayerState.Alive;

    private PlayerMovement movement;
    private PlayerInteraction interaction;
    private PlayerSkills skills;
    private Animator animator;
    private Renderer[] renderers;
    public PhotonView view;


    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        interaction = GetComponent<PlayerInteraction>();
        skills = GetComponent<PlayerSkills>();
        animator = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void DieAndBecomeGhost()
    {
        if (currentState != PlayerState.Alive) return;

        currentState = PlayerState.Dead;
        animator.SetTrigger("getHit");

        if (movement != null) movement.enabled = false;
        if (interaction != null) interaction.enabled = false;
        if (skills != null) skills.enabled = false;

        StartCoroutine(BecomeGhostAfterDelay(5f));
    }

    private System.Collections.IEnumerator BecomeGhostAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentState = PlayerState.Ghost;

        if (movement != null) movement.enabled = true;

        ApplyGhostVisuals(true);

        // Etkileşimi kalıcı olarak kapalı bırak
        if (interaction != null) interaction.enabled = false;
        if (skills != null) skills.enabled = false;
    }

    private void ApplyGhostVisuals(bool ghost)
    {
        SkinnedMeshRenderer rend = GetComponentInChildren<SkinnedMeshRenderer>();
        //    foreach (var rend in renderers)
        //    {
        foreach (var mat in rend.materials)
        {
            /*    Color c = mat.color;
                c.a = ghost ? 0.3f : 1f;
                mat.color = c;
*/
            if (ghost)
            {
                //    mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                // URP Lit shader ayarları
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");

                Color ghostColor = new Color(1f, 1f, 1f, 0.3f); // Beyaz + %30 şeffaf
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", ghostColor);
                else
                    mat.color = ghostColor;

                mat.SetFloat("_Surface", 1); // Transparent
                mat.SetFloat("_Blend", 0);   // Alpha blend
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 3000;
            }
        }
        //   }

        // Hayaleti Watcher ve NPC collider’larıyla çarpıştırma
        Collider myCol = GetComponent<Collider>();
        if (myCol != null)
        {
            foreach (var other in FindObjectsOfType<Collider>())
            {
                if (other.gameObject.CompareTag("Watcher") || other.gameObject.CompareTag("NPC"))
                {
                    Physics.IgnoreCollision(myCol, other, true); // çarpışma yok
                }
            }
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true; // ✅ Gravity açık kalsın
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.velocity = Vector3.zero; // düşme momentumu varsa sıfırla
        }

        if (ghost && animator != null)
        {
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Düz ayağa kaldır
            animator.Play("StaticIdle");
            if (photonView.IsMine)
                UIManager.Instance.SetGhostIcon(true);
        }
    }

}
