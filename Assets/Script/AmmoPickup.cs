using Unity.Netcode;
using UnityEngine;

public class AmmoPickup : NetworkBehaviour {
    
    private float _spawnTime;

    public override void OnNetworkSpawn() {
        _spawnTime = Time.time;
    }

    private void OnTriggerEnter(Collider other) {

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