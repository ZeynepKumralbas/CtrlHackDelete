using Photon.Pun;
using System.Collections;
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

        private IEnumerator Start()
        {
            // RandomColorSelecter sahneye instantiate edildiyse onu bekle
            yield return new WaitUntil(() => RandomColorSelecter.Instance != null);

            AssignNewColor(); // Sahiplik kontrolüne gerek yok artık
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

            //    float[] possibleX = { 0f, 0.205078125f, 0.41015625f, 0.63895625f };

            /*    int randomRange = RandomColorSelecter.Instance.xOffsets.Length;

                int index = Random.Range(0, randomRange);
                offsetX = RandomColorSelecter.Instance.xOffsets[index];
                offsetY = RandomColorSelecter.Instance.yOffsets[index];
            */
            List<Vector2> availableOffsets = RandomColorSelecter.Instance.AvailableColorOffsets;
            Vector2 selectedOffset = availableOffsets[Random.Range(0, availableOffsets.Count)];

            offsetX = selectedOffset.x;
            offsetY = selectedOffset.y;
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
