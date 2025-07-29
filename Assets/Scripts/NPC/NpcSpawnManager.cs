using System.Collections;
using UnityEngine;
using Photon.Pun;

public class NpcSpawnManager : MonoBehaviourPun
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float respawnDelay = 1f;

    public static NpcSpawnManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RespawnNpc()
    {
        StartCoroutine(RespawnAfterDelay(respawnDelay));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (PhotonNetwork.IsMasterClient)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            PhotonNetwork.Instantiate(npcPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
