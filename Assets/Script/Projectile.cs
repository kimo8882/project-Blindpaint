using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour {
    
    [Header("Settings")]
    public float projectileSpeed = 10f;
    public float maxProjectileDistance = 15f;
    public ulong shooterId; 

    private Vector3 _spawnPosition;

    void OnEnable() {
        _spawnPosition = transform.position;
    }

    void Update() {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;

        if (Vector3.Distance(_spawnPosition, transform.position) >= maxProjectileDistance) {
            ProjectilePool.Instance.ReturnToPool(this);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.isTrigger) return; 

        Player enemyPlayer = other.GetComponent<Player>();
        
        if (enemyPlayer != null) {
            // Ignore our own body
            if (enemyPlayer.OwnerClientId == this.shooterId) return; 

            // THE FIX: Only the Server is allowed to calculate the math. 
            // This prevents a glitch where both computers calculate the hit and you take double damage!
            if (NetworkManager.Singleton.IsServer) {
                enemyPlayer.TakeDamageServerRpc(1, this.shooterId); 
            }
        }

        ProjectilePool.Instance.ReturnToPool(this);
    }
}