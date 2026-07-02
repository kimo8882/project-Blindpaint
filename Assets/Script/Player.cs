using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{

    [Header("UI & Effects")]
    [SerializeField] private int _maxHealth = 4;
    [SerializeField] private GameObject _deathEffect, _hitEffect;
    [SerializeField] private HealthBar _healthbar;
    [SerializeField] private GameObject _deathScreenUI;

    [Header("Loot & Rewards")]
    [SerializeField] private GameObject _ammoPickupPrefab;

    [Header("Player Audio")]
    [SerializeField] private AudioClip _gettingHitSound;
    [SerializeField] private AudioClip _hitMarkerSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _healSound;
    [SerializeField] private DamageScreenEffect _damageScreenEffect;
    private AudioSource _audioSource;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> injectors = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> kills = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Awake()
    {
        if (_healthbar == null) _healthbar = GetComponentInChildren<HealthBar>();
        _audioSource = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        if (_healthbar != null) _healthbar.UpdateHealthBar(_maxHealth, currentHealth.Value);
        isDead.OnValueChanged += HandleDeathState;

        if (IsOwner)
        {
            CameraController cam = Camera.main.GetComponent<CameraController>();
            if (cam != null) cam.target = this.transform;
        }

        if (IsServer)
        {
            if (SpawnManager.Instance != null)
            {
                Vector3 randomSpot = SpawnManager.Instance.GetRandomSpawnPosition();
                TeleportClientRpc(randomSpot);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        isDead.OnValueChanged -= HandleDeathState;
    }

    void Update()
    {
        if (_healthbar != null) _healthbar.UpdateHealthBar(_maxHealth, currentHealth.Value);

        // NEW: Update the paint splatters ONLY on the local player's screen
        if (IsOwner && _damageScreenEffect != null)
        {
            _damageScreenEffect.UpdateDamageScreen(currentHealth.Value, _maxHealth);
        }
    }

    private void HandleDeathState(bool wasDead, bool isNowDead)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = !isNowDead;
        foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>()) m.enabled = !isNowDead;
        foreach (SkinnedMeshRenderer sm in GetComponentsInChildren<SkinnedMeshRenderer>()) sm.enabled = !isNowDead;

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = !isNowDead;
        if (_healthbar != null) _healthbar.gameObject.SetActive(!isNowDead);
        if (IsOwner && _deathScreenUI != null) _deathScreenUI.SetActive(isNowDead);
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage, ulong shooterId)
    {
        if (isDead.Value) return;

        currentHealth.Value -= damage;

        // Check if this specific bullet was the killing blow!
        bool isFatalShot = currentHealth.Value <= 0;

        // Send the visual effect to everyone, but ONLY play the audio if they are surviving
        SpawnHitEffectClientRpc(!isFatalShot);
        PlayHitMarkerClientRpc(shooterId);

        if (isFatalShot)
        {
            currentHealth.Value = 0;

            Die(); // The death sound will play here instead!
            DropLoot();
            RewardShooter(shooterId);
        }
    }

    private void DropLoot()
    {
        if (_ammoPickupPrefab != null)
        {
            GameObject pickup = Instantiate(_ammoPickupPrefab, transform.position, Quaternion.identity);
            pickup.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void RewardShooter(ulong shooterId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(shooterId, out NetworkClient client))
        {
            Player shooterPlayer = client.PlayerObject.GetComponent<Player>();
            if (shooterPlayer != null)
            {
                shooterPlayer.kills.Value++;
                if (shooterPlayer.kills.Value % 5 == 0) shooterPlayer.injectors.Value++;
            }
        }
    }

    private void Die()
    {
        // 1. VISUAL: Everyone sees the body drop
        SpawnDeathEffectClientRpc();
        isDead.Value = true;

        // 2. AUDIO: Only the dead player hears their own death scream
        ClientRpcParams victimParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { OwnerClientId } } };
        PlayDeathSoundClientRpc(victimParams);
    }

    [ClientRpc]
    private void SpawnHitEffectClientRpc(bool playHitSound)
    {
        if (_hitEffect != null) Instantiate(_hitEffect, transform.position, Quaternion.identity);

        if (playHitSound && IsOwner)
        {
            if (_gettingHitSound != null && _audioSource != null) _audioSource.PlayOneShot(_gettingHitSound);
        }
    }

    [ClientRpc]
    private void SpawnDeathEffectClientRpc()
    {
        if (_deathEffect != null) Instantiate(_deathEffect, transform.position, Quaternion.Euler(-90, 0, 0));
    }


    // --- AUDIO RPCs (Targeted to strictly ONE computer) ---
    [ClientRpc]
    private void PlayGettingHitSoundClientRpc(ClientRpcParams rpcParams = default)
    {
        if (_gettingHitSound != null && _audioSource != null) _audioSource.PlayOneShot(_gettingHitSound);
    }

    [ClientRpc]
    private void PlayHitMarkerClientRpc(ulong shooterId)
    {
        // Check if THIS client instance belongs to the player who actually fired the shot
        if (NetworkManager.Singleton.LocalClientId == shooterId)
        {
            // Plays audio directly relative to the main camera transform so it behaves like 2D UI audio
            if (_hitMarkerSound != null) AudioSource.PlayClipAtPoint(_hitMarkerSound, Camera.main.transform.position);
        }
    }

    [ClientRpc]
    private void PlayDeathSoundClientRpc(ClientRpcParams rpcParams = default)
    {
        if (_deathSound != null && _audioSource != null) _audioSource.PlayOneShot(_deathSound);
    }

    [ClientRpc]
    private void PlayHealSoundClientRpc(ClientRpcParams rpcParams = default)
    {
        if (_healSound != null && _audioSource != null) _audioSource.PlayOneShot(_healSound);
    }


    public void ClickRespawn()
    {
        if (IsOwner) RespawnServerRpc();
    }

    [ServerRpc]
    private void RespawnServerRpc()
    {
        currentHealth.Value = _maxHealth;
        isDead.Value = false;

        if (SpawnManager.Instance != null)
        {
            Vector3 randomSpot = SpawnManager.Instance.GetRandomSpawnPosition();
            TeleportClientRpc(randomSpot);
        }
        else
        {
            TeleportClientRpc(new Vector3(0, 2, 0));
        }

        PlayerGun gun = GetComponentInChildren<PlayerGun>();
        if (gun != null) gun.RefillClientRpc();
    }

    [ServerRpc]
    public void UseInjectorServerRpc()
    {
        if (injectors.Value > 0 && currentHealth.Value < _maxHealth && !isDead.Value)
        {
            injectors.Value--;
            currentHealth.Value = _maxHealth;

            ClientRpcParams healerParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { OwnerClientId } } };
            PlayHealSoundClientRpc(healerParams);
        }
    }

    [ClientRpc]
    private void TeleportClientRpc(Vector3 newPosition)
    {
        newPosition.y += 1.0f;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            transform.position = newPosition;
            cc.enabled = true;
        }
        else
        {
            transform.position = newPosition;
        }
    }
}