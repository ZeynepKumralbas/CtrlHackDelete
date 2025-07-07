using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    void OnGUI()
    {
        // ❶  NetworkManager yoksa temiz mesaj ver
        if (NetworkManager.Singleton == null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "NetworkManager bulunamadı.");
            return;
        }

        // ❷  Henüz bağlantı kurulmadıysa seçim butonlarını çiz
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUI.Button(new Rect(10, 40, 150, 30), "Start as Host"))
                NetworkManager.Singleton.StartHost();

            if (GUI.Button(new Rect(10, 80, 150, 30), "Start as Client"))
                NetworkManager.Singleton.StartClient();

            if (GUI.Button(new Rect(10, 120, 150, 30), "Start as Server"))
                NetworkManager.Singleton.StartServer();
        }
        // ❸  Bağlantı kurulmuşsa bilgi göster
        else
        {
            string role = NetworkManager.Singleton.IsHost ? "Host"
                : NetworkManager.Singleton.IsServer ? "Server" : "Client";
            GUI.Label(new Rect(10, 10, 300, 20), $"Ağ başlatıldı: {role}");
        }
    }
}
