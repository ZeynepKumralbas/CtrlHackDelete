/*
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BufferedNetworkTransform : MonoBehaviourPun, IPunObservable
{
    private struct NetworkFrame
    {
        public Vector3 position;
        public Quaternion rotation;
        public double timestamp;
    }

    [SerializeField] private float interpolationBackTime = 0.1f; // 100 ms gecikme ile oynatma
    private List<NetworkFrame> frameBuffer = new List<NetworkFrame>();

    private Vector3 latestVelocity;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (photonView.IsMine) return; // Sadece remote için çalışır

        double renderTime = PhotonNetwork.Time - interpolationBackTime;

        // Buffer boşsa çık
        if (frameBuffer.Count == 0) return;

        // 1. Bufferda renderTime’dan küçük en büyük frame’ı bul
        int i = 0;
        while (i < frameBuffer.Count - 1 && frameBuffer[i + 1].timestamp <= renderTime)
            i++;

        // 2. Eğer renderTime frameBuffer’ın en sonunda ise son frame’a sabitle
        if (i == frameBuffer.Count - 1)
        {
            transform.position = frameBuffer[i].position;
            transform.rotation = frameBuffer[i].rotation;
            return;
        }

        // 3. Frame i ile i+1 arasında interpolation yapılacak
        NetworkFrame older = frameBuffer[i];
        NetworkFrame newer = frameBuffer[i + 1];

        double timeDiff = newer.timestamp - older.timestamp;
        if (timeDiff <= 0.0001) timeDiff = 0.0001; // sıfıra bölünmeyi engelle

        float t = (float)((renderTime - older.timestamp) / timeDiff);
        t = Mathf.Clamp01(t);

        // 4. Lineer interpolation yap
        transform.position = Vector3.Lerp(older.position, newer.position, t);
        transform.rotation = Quaternion.Slerp(older.rotation, newer.rotation, t);

        // 5. Bufferdan eski frame’leri sil (zamanı geçmiş olanlar)
        frameBuffer.RemoveRange(0, i);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Local player pozisyon ve rotasyon gönder
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(PhotonNetwork.Time);
        }
        else
        {
            // Remote player’dan pozisyon ve rotasyon al
            Vector3 pos = (Vector3)stream.ReceiveNext();
            Quaternion rot = (Quaternion)stream.ReceiveNext();
            double timestamp = (double)stream.ReceiveNext();

            // Yeni frame oluştur ve buffer’a ekle
            NetworkFrame frame = new NetworkFrame
            {
                position = pos,
                rotation = rot,
                timestamp = timestamp
            };

            // Frame buffer’a zaman sırasına göre ekle
            int index = frameBuffer.FindIndex(f => f.timestamp > timestamp);
            if (index == -1)
                frameBuffer.Add(frame);
            else
                frameBuffer.Insert(index, frame);
        }
    }
}
*/

using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BufferedNetworkTransform : MonoBehaviourPun, IPunObservable
{
    private struct NetworkFrame
    {
        public Vector3 position;
        public Quaternion rotation;
        public double timestamp;
    }

    [SerializeField] private float interpolationBackTime = 0.12f; // 120 ms gecikmeli oynatma
    private List<NetworkFrame> frameBuffer = new List<NetworkFrame>();

    [SerializeField] private float snapThreshold = 0.001f; // Çok küçük farklar için snap
    [SerializeField] private bool enableDebugDraw = false;

    private void Update()
    {
        if (photonView.IsMine) return; // Sadece remote için çalışır

        double renderTime = PhotonNetwork.Time - interpolationBackTime;

        if (frameBuffer.Count == 0) return;

        // Frame seçim
        int i = 0;
        while (i < frameBuffer.Count - 1 && frameBuffer[i + 1].timestamp <= renderTime)
            i++;

        if (i == frameBuffer.Count - 1)
        {
            transform.position = frameBuffer[i].position;
            transform.rotation = frameBuffer[i].rotation;
            return;
        }

        NetworkFrame older = frameBuffer[i];
        NetworkFrame newer = frameBuffer[i + 1];

        double timeDiff = newer.timestamp - older.timestamp;
        if (timeDiff < 0.0001) timeDiff = 0.0001;

        float t = (float)((renderTime - older.timestamp) / timeDiff);
        t = Mathf.Clamp01(t);

        // Interpolation
        Vector3 interpolatedPos = Vector3.Lerp(older.position, newer.position, t);
        Quaternion interpolatedRot = Quaternion.Slerp(older.rotation, newer.rotation, t);

        // Çok küçük fark varsa doğrudan snaple
        if ((transform.position - interpolatedPos).sqrMagnitude < snapThreshold * snapThreshold)
        {
            transform.position = interpolatedPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, interpolatedPos, Time.smoothDeltaTime * 20f);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, interpolatedRot, Time.smoothDeltaTime * 20f);

        // Eski frame'leri sil
        if (i > 0) frameBuffer.RemoveRange(0, i);

        if (enableDebugDraw)
        {
            Debug.DrawLine(transform.position + Vector3.up * 2f, interpolatedPos + Vector3.up * 2f, Color.cyan, 0.1f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(PhotonNetwork.Time);
        }
        else
        {
            Vector3 pos = (Vector3)stream.ReceiveNext();
            Quaternion rot = (Quaternion)stream.ReceiveNext();
            double timestamp = (double)stream.ReceiveNext();

            NetworkFrame frame = new NetworkFrame
            {
                position = pos,
                rotation = rot,
                timestamp = timestamp
            };

            // Zamana göre sıraya ekle
            int index = frameBuffer.FindIndex(f => f.timestamp > timestamp);
            if (index == -1)
                frameBuffer.Add(frame);
            else
                frameBuffer.Insert(index, frame);
        }
    }
}
