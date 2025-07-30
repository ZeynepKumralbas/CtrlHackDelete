using UnityEngine;

[ExecuteAlways]
public class GhostMaterialDebugger : MonoBehaviour
{
    public SkinnedMeshRenderer[] parts;
    [Range(0f, 1f)] public float alpha = 0.3f;
    [ColorUsage(true, true)] public Color emissionColor = Color.white;

    void Update()
    {
        foreach (var rend in parts)
        {
            var mats = rend.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                var mat = mats[i];
                if (mat == null) continue;

                if (mat.HasProperty("_BaseColor"))
                {
                    Color baseColor = mat.GetColor("_BaseColor");
                    baseColor.a = alpha;
                    mat.SetColor("_BaseColor", baseColor);
                }

                mat.SetFloat("_Surface", 1);
                mat.SetFloat("_Blend", 0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 3000;

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", emissionColor * alpha);
                }
            }
        }
    }
}
