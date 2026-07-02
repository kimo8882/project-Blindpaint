using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    void Start()
    {
        // Give the network manager a split second to wake up before we try to connect
        Invoke("AutoConnect", 0.1f);
    }

    private void AutoConnect()
    {
#if UNITY_EDITOR
        // --- 1. WE ARE IN THE UNITY EDITOR ---
        Debug.Log("Main Editor detected! Auto-Starting as HOST...");
        NetworkManager.Singleton.StartHost();
#else
        // --- 2. WE ARE IN THE BUILT .EXE GAME ---
        Debug.Log("Built Game detected! Auto-Starting as CLIENT...");
        NetworkManager.Singleton.StartClient();
#endif
    }
}