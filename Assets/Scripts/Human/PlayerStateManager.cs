using UnityEngine;
using Photon.Pun;
using YourNamespaceHere;

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
    public float alpha = 0.3f;
    public float emissionColor = 0.5f;


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
        /*    SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer rend in renderers)
            {
                Material[] newMaterials = new Material[rend.materials.Length];

                for (int i = 0; i < rend.materials.Length; i++)
                {
                    Material original = rend.materials[i];
                    Material instanceMat = new Material(original); // ✨ instance

                    instanceMat.shader = Shader.Find("Universal Render Pipeline/Lit");

                    if (ghost)
                    {
                        Color ghostWhite = new Color(1f, 1f, 1f, 0.3f);

                        if (instanceMat.HasProperty("_BaseColor"))
                            instanceMat.SetColor("_BaseColor", ghostWhite);
                        else
                            instanceMat.color = ghostWhite;

                        instanceMat.SetFloat("_Surface", 1); // Transparent
                        instanceMat.SetFloat("_Blend", 0);   // Alpha blend
                        instanceMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        instanceMat.renderQueue = 3000;

                        // Glow
                        instanceMat.EnableKeyword("_EMISSION");
                        instanceMat.SetColor("_EmissionColor", Color.white * 1.5f);
                    }

                    newMaterials[i] = instanceMat;
                }

                rend.materials = newMaterials; // 🚀 Her parçaya uygula
            }*/
        

    // ⬇️ Robotun kendi UV offset'ini bul
    Vector2 offset = Vector2.zero;
    foreach (Renderer r in GetComponentsInChildren<Renderer>())
    {
        foreach (Material mat in r.materials)
        {
            if (mat.HasProperty("_UV_Offset"))
            {
                offset = mat.GetVector("_UV_Offset");
                Debug.Log("offset:" + offset);
                goto Found;
            }
        }
    }

        
Found:;

    foreach (SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>())
    {
        Material[] newMaterials = new Material[rend.materials.Length];

        for (int i = 0; i < rend.materials.Length; i++)
        {
            Material original = rend.materials[i];
            Material instanceMat = new Material(original); // Yeni material oluştur

            instanceMat.shader = Shader.Find("Universal Render Pipeline/Lit");

            // 💡 Renk offsetini koru
            if (instanceMat.HasProperty("_UV_Offset"))
                instanceMat.SetVector("_UV_Offset", offset);

                if (ghost)
                {
                    // Şeffaflık ve glow
                    instanceMat.SetFloat("_Surface", 1); // Transparent
                    instanceMat.SetFloat("_Blend", 0);   // Alpha blend
                    instanceMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    instanceMat.renderQueue = 3000;



                    if (instanceMat.HasProperty("_BaseColor"))
                    {
                        Color c = instanceMat.GetColor("_BaseColor");
                        c.a = alpha;
                        instanceMat.SetColor("_BaseColor", c);

                        instanceMat.EnableKeyword("_EMISSION");
                        instanceMat.SetColor("_EmissionColor", c * emissionColor);
                    }
            }

            newMaterials[i] = instanceMat;
        }

        rend.materials = newMaterials;
    }

    // ... devamı zaten sende var







    /*    if (ghost && photonView.IsMine)
                {
                    if (ModularRobotRandomizer.Instance != null)
                        ModularRobotRandomizer.Instance.ApplyWhiteGhostLook();
                }*/


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
