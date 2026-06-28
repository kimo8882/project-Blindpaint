using Unity.Netcode;
using UnityEngine;

public class AmmoPickup : NetworkBehaviour {
    
    private float _spawnTime;

    // This runs the exact moment the bottle appears in the world
    public override void OnNetworkSpawn() {
        _spawnTime = Time.time;
    }

    private void OnTriggerEnter(Collider other) {
        // THE FIX: Prevent anyone from picking it up for the first 0.5 seconds!
        // This stops the dying player from instantly eating their own loot.
        if (Time.time < _spawnTime + 0.5f) return;

        Player player = other.GetComponent<Player>();
        
        if (player != null && player.IsOwner && !player.isDead.Value) {
            RequestPickupServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPickupServerRpc(ServerRpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client)) {
            PlayerGun gun = client.PlayerObject.GetComponentInChildren<PlayerGun>();
            if (gun != null) {
                gun.RefillClientRpc();
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}