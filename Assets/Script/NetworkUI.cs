using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        
        // SAFETY CHECK: Only draw the buttons if the NetworkManager actually exists yet!
        if (NetworkManager.Singleton != null)
        {
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (GUILayout.Button("Start Host", GUILayout.Width(200), GUILayout.Height(50))) 
                    NetworkManager.Singleton.StartHost();
                    
                if (GUILayout.Button("Start Client", GUILayout.Width(200), GUILayout.Height(50))) 
                    NetworkManager.Singleton.StartClient();
            }
        }
        
        GUILayout.EndArea();
    }
}