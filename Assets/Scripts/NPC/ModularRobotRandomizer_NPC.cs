using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace YourNamespaceHere
{
    public class ModularRobotRandomizer_NPC : MonoBehaviourPun, IPunObservable
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

        private void Awake()
        {
            OrganizeRobotParts();
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                AssignNewColor(); // Sadece sahibi rengi belirler
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && photonView.IsMine)
            {
                AssignNewColor();
            }
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

        public void AssignNewColor()
        {
            if (colorAssigned) return;

            float[] possibleX = { 0f, 0.205078125f, 0.41015625f, 0.63895625f };

            offsetX = possibleX[Random.Range(0, possibleX.Length)];
            offsetY = Random.Range(0, 32) * 0.03125f;

            colorAssigned = true;
            ApplyOffsetToAllParts(offsetX, offsetY);
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(offsetX);
                stream.SendNext(offsetY);
            }
            else
            {
                offsetX = (float)stream.ReceiveNext();
                offsetY = (float)stream.ReceiveNext();

                ApplyOffsetToAllParts(offsetX, offsetY);
            }
        }
    }
}
