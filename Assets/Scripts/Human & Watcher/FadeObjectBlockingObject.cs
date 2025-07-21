using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectBlockingObject : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform target;
    [SerializeField] private Camera cam;

    [SerializeField]
    [Range(0f, 1f)]
    private float fadedAlpha = 0.33f;

    [SerializeField] private bool retainShadows = true;
    [SerializeField] private Vector3 targetPositionOffset = Vector3.up;
    [SerializeField] private float fadeSpeed = 1.0f;

    [Header("Read Only Data")]
    [SerializeField] private List<FadingObject> objectBlockingView = new List<FadingObject>();

    private Dictionary<FadingObject, Coroutine> runningCoroutines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] raycastHits = new RaycastHit[10];

    public PhotonView view;

    private void Start()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }
        if(view == null)
        {
            view = GetComponent<PhotonView>();
        }
        if (view.IsMine)
        {
            StartCheckForObjects();
        }
    }
    private void StartCheckForObjects()
    {
        StartCoroutine(CheckForObjects());
    }
    private IEnumerator CheckForObjects()
    {
        while (true)
        {
            int hits = Physics.RaycastNonAlloc(
                cam.transform.position,
                (target.transform.position + targetPositionOffset - cam.transform.position).normalized,
                raycastHits,
                Vector3.Distance(cam.transform.position, target.transform.position + targetPositionOffset),
                layerMask
                );

            if (hits > 0)
            {
                for(int i = 0; i < hits; i++)
                {
                    FadingObject fadingObject = GetFadingObjectFromHit(raycastHits[i]);

                    if(fadingObject != null && !objectBlockingView.Contains(fadingObject))
                    {
                        if (runningCoroutines.ContainsKey(fadingObject))
                        {
                            if(runningCoroutines[fadingObject] != null)
                            {
                                StopCoroutine(runningCoroutines[fadingObject]);
                            }
                            runningCoroutines.Remove(fadingObject);
                        }
                        runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        objectBlockingView.Add(fadingObject);
                    }

                }

            }
            FadeObjectsNoLongerBeingHit();

            ClearHits();
            yield return null;
        }
    }
    private void FadeObjectsNoLongerBeingHit()
    {

        List<FadingObject> objectsToRemove = new List<FadingObject>(objectBlockingView.Count);

        foreach (FadingObject fadingObject in objectBlockingView)
        {
            bool objectIsBeingHit = false;
            for (int i = 0; i < raycastHits.Length; i++)
            {
                FadingObject hitFadingObject = GetFadingObjectFromHit(raycastHits[i]);
                if(hitFadingObject != null && fadingObject == hitFadingObject)
                {
                    objectIsBeingHit = true;
                    break;
                }
            }
            if(!objectIsBeingHit)
            {
                if (runningCoroutines.ContainsKey(fadingObject))
                {
                    if(runningCoroutines[fadingObject] != null)
                    {
                        StopCoroutine(runningCoroutines[fadingObject]);
                    }
                    runningCoroutines.Remove(fadingObject);
                }
                runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }
        /**********************/
        Debug.Log("deneme");
        /**********************/
        foreach (FadingObject removeObject in objectsToRemove)
        {
            objectBlockingView.Remove(removeObject);
            Debug.Log("Silme deneme");
        }
    }
    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        foreach (Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", retainShadows);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        float time = 0;
        while (fadingObject.materials[0].color.a > fadedAlpha)
        {
            foreach(Material material in fadingObject.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadingObject.initialAlpha, fadedAlpha, time * fadeSpeed)
                        );
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
        if (runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }
    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        float time = 0;
        while (fadingObject.materials[0].color.a < fadingObject.initialAlpha)
        {
            foreach (Material material in fadingObject.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadedAlpha, fadingObject.initialAlpha, time * fadeSpeed)
                        );
                }
            }
            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
        if (runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }
    private void ClearHits()
    {
        System.Array.Clear(raycastHits, 0, raycastHits.Length);
    }
    private FadingObject GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadingObject>() : null;
    }
}
