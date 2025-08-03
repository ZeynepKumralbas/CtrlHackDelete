using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourNamespaceHere
{
    public class ModularRobotRandomizer_NPC1 : MonoBehaviour
    {
        private List<GameObject> heads = new List<GameObject>();
        private List<GameObject> bodies = new List<GameObject>();
        private List<GameObject> armsL = new List<GameObject>();
        private List<GameObject> armsR = new List<GameObject>();
        private List<GameObject> legsL = new List<GameObject>();
        private List<GameObject> legsR = new List<GameObject>();
        private List<GameObject> activeParts = new List<GameObject>();

        [SerializeField] private string materialNameToModify = "M_AtlasOffset";
        [SerializeField] private Material materialToModify;

        private float offsetX;
        private float offsetY;

        private bool colorAssigned = false;

        public float[] xOffsets = new float[2];
        public float[] yOffsets = new float[2];
        public List<Vector2> AvailableColorOffsets = new List<Vector2>();
        private float[] possibleValues = { 0f, 0.205078125f, 0.41015625f, 0.63895625f };
         private float previousX = -1f;
        private float previousY = -1f;



        private void Awake()
        {
            OrganizeRobotParts();
        }

        private void Start()
        {
            OffsetIdentifier();
            GenerateAllOffsetCombinations();
            RandomizeMaterialOffsets();
            ApplyMaterialOffsets();
        }

        private void OrganizeRobotParts()
        {
            foreach (Transform part in transform)
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

        public void OffsetIdentifier()
        {
            if (xOffsets.Length == yOffsets.Length)
            {
                for (int i = 0; i < xOffsets.Length; i++)
                {
                    xOffsets[i] = possibleValues[Random.Range(0, possibleValues.Length)];
                    yOffsets[i] = Random.Range(0, 32) * 0.03125f;
                }
            }
        }
        private void GenerateAllOffsetCombinations()
        {
            AvailableColorOffsets.Clear();

            foreach (float x in xOffsets)
            {
                foreach (float y in yOffsets)
                {
                    AvailableColorOffsets.Add(new Vector2(x, y));
                }
            }
        }

        public void RandomizeMaterialOffsets()
        {
            int randomRange = xOffsets.Length;

            float newX, newY;
            int attempt = 0;
            const int maxAttempts = 10; // Sonsuz d�ng�den ka��nmak i�in g�venlik s�n�r�

            do
            {
                int index = Random.Range(0, randomRange);
                newX = xOffsets[index];
                newY = yOffsets[index];
                attempt++;
            }
            while ((newX == previousX && newY == previousY) && attempt < maxAttempts);

            previousX = newX;
            previousY = newY;

            offsetX = newX;
            offsetY = newY;
        }

        void ApplyMaterialOffsets()
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

        private void ApplyOffsetToAllParts(float x, float y)
        {
            foreach (GameObject part in activeParts)
            {
                if (part == null) continue;

                SkinnedMeshRenderer renderer = part.GetComponent<SkinnedMeshRenderer>();
                if (renderer == null) continue;

                int index = GetMaterialIndex(renderer);
                if (index == -1) continue;

                Material mat = renderer.materials[index];
                mat.SetVector("_UV_Offset", new Vector2(x, y));
            }
        }

        private int GetMaterialIndex(SkinnedMeshRenderer renderer)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                Material mat = renderer.materials[i];
                if ((materialToModify != null && mat == materialToModify) || mat.name.Contains(materialNameToModify))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
