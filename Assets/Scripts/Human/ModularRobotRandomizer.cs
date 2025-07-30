using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace YourNamespaceHere
{
    public class ModularRobotRandomizer : MonoBehaviourPun
    {
        public static ModularRobotRandomizer Instance;

        private List<GameObject> heads = new List<GameObject>();
        private List<GameObject> bodies = new List<GameObject>();
        private List<GameObject> armsL = new List<GameObject>();
        private List<GameObject> armsR = new List<GameObject>();
        private List<GameObject> legsL = new List<GameObject>();
        private List<GameObject> legsR = new List<GameObject>();

        private List<GameObject> activeParts = new List<GameObject>();

        [SerializeField] private string materialNameToModify = "M_AtlasOffset"; // Material name
        [SerializeField] private Material materialToModify; // Optional material reference

        //Multiplayer
        public PhotonView view;

        private float previousX = -1f;
        private float previousY = -1f;

        private void Awake()
        {
            OrganizeRobotParts(); //Robot par�alar�n� listelere yerle�tir
        }

        private void Start()
        {
            Instance = this;

            if (view.IsMine)
            {
                // �lk rastgele rengi sadece kendi objen i�in olu�tur ve t�m oyunculara g�nder
                RandomizeMaterialOffsets();
            }
        }

        private void Update()
        {
            if (!view.IsMine)
                return;

            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                RandomizeMaterialOffsets();
            }*/
        }

        private void OrganizeRobotParts()
        {
            Transform parent = this.gameObject.transform;

            foreach (Transform part in parent)
            {
                string partName = part.name;

                if (partName.Contains("Head")) heads.Add(part.gameObject);
                else if (partName.Contains("Body")) bodies.Add(part.gameObject);
                else if (partName.Contains("Arm.L")) armsL.Add(part.gameObject);
                else if (partName.Contains("Arm.R")) armsR.Add(part.gameObject);
                else if (partName.Contains("Leg.L")) legsL.Add(part.gameObject);
                else if (partName.Contains("Leg.R")) legsR.Add(part.gameObject);

                activeParts.Add(part.gameObject);
            }
        }

        /*public void RandomizeMaterialOffsets()
        {
            //Renk de�i�ikli�ini t�m oyunculara bildir
            view.RPC("ApplyMaterialOffsets", RpcTarget.AllBuffered,
                RandomColorSelecter.Instance.xOffsets[Random.Range(0, RandomColorSelecter.Instance.xOffsets.Length)],
                RandomColorSelecter.Instance.yOffsets[Random.Range(0, RandomColorSelecter.Instance.yOffsets.Length)]);
        }*/
        public void RandomizeMaterialOffsets()
        {
            int randomRange = RandomColorSelecter.Instance.xOffsets.Length;

            float newX, newY;
            int attempt = 0;
            const int maxAttempts = 10; // Sonsuz d�ng�den ka��nmak i�in g�venlik s�n�r�

            do
            {
                int index = Random.Range(0, randomRange);
                newX = RandomColorSelecter.Instance.xOffsets[index];
                newY = RandomColorSelecter.Instance.yOffsets[index];
                attempt++;
            }
            while ((newX == previousX && newY == previousY) && attempt < maxAttempts);

            previousX = newX;
            previousY = newY;

            view.RPC("ApplyMaterialOffsets", RpcTarget.AllBuffered, newX, newY);
        }
        [PunRPC]
        void ApplyMaterialOffsets(float offsetX, float offsetY)
        {
            Debug.Log("x: " + offsetX + "\n" + "y: " + offsetY);

            foreach (GameObject part in activeParts)
            {
                if (part != null)
                {
                    SkinnedMeshRenderer renderer = part.GetComponent<SkinnedMeshRenderer>();
                    if (renderer != null)
                    {
                        //Robot par�alarinin rendererlarinin index degerlerini al
                        int materialIndex = GetMaterialIndex(renderer);
                        if (materialIndex != -1)
                        {
                            Material mat = renderer.materials[materialIndex];
                            mat.SetVector("_UV_Offset", new Vector2(offsetX, offsetY));
                        }
                    }
                }
            }
        }

        private int GetMaterialIndex(SkinnedMeshRenderer renderer)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                Material mat = renderer.materials[i];

                //Dogru renk skalasi eslesiyorsa eslesen index degerini dondur
                if ((materialToModify != null && mat == materialToModify) || mat.name.Contains(materialNameToModify))
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetGhostMaterialTransparency(bool isGhost)
        {
            foreach (GameObject part in activeParts)
            {
                if (part != null)
                {
                    SkinnedMeshRenderer renderer = part.GetComponent<SkinnedMeshRenderer>();
                    if (renderer != null)
                    {
                        foreach (Material mat in renderer.materials)
                        {
                            if (isGhost)
                            {
                                // URP şeffaflık ayarları
                                mat.shader = Shader.Find("Universal Render Pipeline/Lit");

                                Color currentColor;
                                if (mat.HasProperty("_BaseColor"))
                                    currentColor = mat.GetColor("_BaseColor");
                                else
                                    currentColor = mat.color;

                                currentColor.a = 0.3f; // 🔹 sadece şeffaflık düşürüldü

                                if (mat.HasProperty("_BaseColor"))
                                    mat.SetColor("_BaseColor", currentColor);
                                else
                                    mat.color = currentColor;

                                mat.SetFloat("_Surface", 1); // Transparent
                                mat.SetFloat("_Blend", 0);   // Alpha blend
                                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                                mat.renderQueue = 3000;
                            }
                        }
                    }
                }
            }
        }

        public void ApplyWhiteGhostLook()
        {
            Debug.Log("ApplyWhiteGhostLook");
            foreach (GameObject part in activeParts)
            {
                if (part != null)
                {
                    SkinnedMeshRenderer renderer = part.GetComponent<SkinnedMeshRenderer>();
                    if (renderer != null)
                    {
                        foreach (Material mat in renderer.materials)
                        {
                            mat.shader = Shader.Find("Universal Render Pipeline/Lit");

                            // Ana rengi saf beyaz + şeffaflık
                            Color ghostWhite = new Color(1f, 1f, 1f, 0.3f); // %30 saydam beyaz

                            if (mat.HasProperty("_BaseColor"))
                                mat.SetColor("_BaseColor", ghostWhite);
                            else
                                mat.color = ghostWhite;

                            // Emission (ışık yayma) açık
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", new Color(1f, 1f, 1f) * 2f); // Parlaklık

                            // URP şeffaflık ayarları
                            mat.SetFloat("_Surface", 1); // Transparent
                            mat.SetFloat("_Blend", 0);   // Alpha
                            mat.renderQueue = 3000;
                            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                        }
                    }
                }
            }
        }

    }
}
