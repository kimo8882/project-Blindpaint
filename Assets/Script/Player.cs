using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour {
    
    [Header("UI & Effects")]
    [SerializeField] private int _maxHealth = 4; 
    [SerializeField] private GameObject _deathEffect, _hitEffect;
    [SerializeField] private HealthBar _healthbar;
    [SerializeField] private GameObject _deathScreenUI; 

    [Header("Loot & Rewards")]
    [SerializeField] private GameObject _ammoPickupPrefab;
    
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> injectors = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> kills = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn() {
        if (_healthbar != null) _healthbar.UpdateHealthBar(_maxHealth, currentHealth.Value);
        isDead.OnValueChanged += HandleDeathState;

        if (IsOwner) {
            CameraController cam = Camera.main.GetComponent<CameraController>();
            if (cam != null) cam.target = this.transform;
        }
    }

    public override void OnNetworkDespawn() {
        isDead.OnValueChanged -= HandleDeathState; 
    }

    void Update() {
        if (_healthbar != null) _healthbar.UpdateHealthBar(_maxHealth, currentHealth.Value);
    }

    private void HandleDeathState(bool wasDead, bool isNowDead) {
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = !isNowDead;
        foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>()) m.enabled = !isNowDead;
        foreach (SkinnedMeshRenderer sm in GetComponentsInChildren<SkinnedMeshRenderer>()) sm.enabled = !isNowDead;
        
        GetComponent<PlayerController>().enabled = !isNowDead;
        if (_healthbar != null) _healthbar.gameObject.SetActive(!isNowDead);

        if (IsOwner && _deathScreenUI != null) {
            _deathScreenUI.SetActive(isNowDead);
        }
    }

    // --- THE NEW DAMAGE & LOOT SYSTEM ---
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage, ulong shooterId) {
        if (isDead.Value) return; 

        currentHealth.Value -= damage;
        SpawnHitEffectClientRpc();

        if (currentHealth.Value <= 0) {
            currentHealth.Value = 0;
            
            Die();
            DropLoot(); 
            RewardShooter(shooterId); 
        }
    }

    private void DropLoot() {
        if (_ammoPickupPrefab != null) {
            GameObject pickup = Instantiate(_ammoPickupPrefab, transform.position, Quaternion.identity);
            pickup.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void RewardShooter(ulong shooterId) {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(shooterId, out NetworkClient client)) {
            Player shooterPlayer = client.PlayerObject.GetComponent<Player>();
            if (shooterPlayer != null) {
                shooterPlayer.kills.Value++; 
                if (shooterPlayer.kills.Value % 5 == 0) {
                    shooterPlayer.injectors.Value++;
                }
            }
        }
    }

    private void Die() {
        SpawnDeathEffectClientRpc();
        isDead.Value = true; 
    }

    [ClientRpc]
    private void SpawnHitEffectClientRpc() {
        if (_hitEffect != null) Instantiate(_hitEffect, transform.position, Quaternion.identity);
    }

    [ClientRpc]
    private void SpawnDeathEffectClientRpc() {
        if (_deathEffect != null) Instantiate(_deathEffect, transform.position, Quaternion.Euler(-90, 0, 0));
    }

    public void ClickRespawn() {
        if (IsOwner) RespawnServerRpc();
    }

    [ServerRpc]
    private void RespawnServerRpc() {
        currentHealth.Value = _maxHealth;
        isDead.Value = false;
        transform.position = new Vector3(0, 2, 0); 

        PlayerGun gun = GetComponentInChildren<PlayerGun>();
        if (gun != null) gun.RefillClientRpc();
    }

    [ServerRpc]
    public void UseInjectorServerRpc() {
        if (injectors.Value > 0 && currentHealth.Value < _maxHealth && !isDead.Value) {
            injectors.Value--;
            currentHealth.Value = _maxHealth;
        }
    }
}