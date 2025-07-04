using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace YourNamespaceHere
{
    public class ModularRobotRandomizer : MonoBehaviourPun
    {
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

        private void Awake()
        {
            OrganizeRobotParts(); //Robot parçalarýný listelere yerleþtir
        }

        private void Start()
        {
            if (view.IsMine)
            {
                // Ýlk rastgele rengi sadece kendi objen için oluþtur ve tüm oyunculara gönder
                RandomizeMaterialOffsets();
            }
        }

        private void Update()
        {
            if (!view.IsMine)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RandomizeMaterialOffsets();
            }
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

        private void RandomizeMaterialOffsets()
        {
            //Rastgele birer offset degeri belirle
            float[] possibleValues = { 0f, 0.205078125f, 0.41015625f };
            float randomX = possibleValues[Random.Range(0, possibleValues.Length)];
            float randomY = Random.Range(0, 32) * 0.03125f;

            //Renk deðiþikliðini tüm oyunculara bildir
            view.RPC("ApplyMaterialOffsets", RpcTarget.AllBuffered, randomX, randomY);
        }

        [PunRPC]
        void ApplyMaterialOffsets(float offsetX, float offsetY)
        {
            foreach (GameObject part in activeParts)
            {
                if (part != null)
                {
                    SkinnedMeshRenderer renderer = part.GetComponent<SkinnedMeshRenderer>();
                    if (renderer != null)
                    {
                        //Robot parçalarinin rendererlarinin index degerlerini al
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
    }
}
