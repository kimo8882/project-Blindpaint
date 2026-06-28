using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode; 

public class PlayerGun : NetworkBehaviour { 
    
    [SerializeField] Transform firingPoint;
    [SerializeField] float firingSpeed = 1f;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 20;
    [SerializeField] private int maxBottles = 3;
    [SerializeField] private float reloadTime = 1.5f; 
    
    public int currentAmmo;
    public int currentBottles;
    private bool isReloading = false;

    private float lastTimeShot = 0;
    private int lastButtonUsed = -1; 

    public override void OnNetworkSpawn() {
        currentAmmo = maxAmmo;
        currentBottles = maxBottles;
    }

    public void Shoot(int buttonIndex) {
        if (isReloading) return;
        
        if (currentAmmo <= 0) {
            Debug.Log("Out of ammo! Press R to reload.");
            return;
        }

        bool isAlternating = (buttonIndex != lastButtonUsed) && (lastButtonUsed != -1);

        if (!isAlternating) {
            if (lastTimeShot + firingSpeed > Time.time) return; 
        }

        lastTimeShot = Time.time;
        lastButtonUsed = buttonIndex; 
        currentAmmo--;
        
        ShootServerRpc(OwnerClientId);
    }

    public void Reload() {
        if (isReloading || currentAmmo == maxAmmo || currentBottles <= 0) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine() {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        currentBottles--;
        currentAmmo = maxAmmo;
        
        isReloading = false;
    }

    [ServerRpc]
    private void ShootServerRpc(ulong shooterId) {
        ShootClientRpc(shooterId);
    }

    [ClientRpc]
    private void ShootClientRpc(ulong shooterId) {
        // THE FIX: Send the ID directly into the Object Pool
        ProjectilePool.Instance.Instantiate(firingPoint.position, firingPoint.rotation, shooterId);
    }

    [ClientRpc]
    public void RefillClientRpc() {
        if (IsOwner) {
            currentAmmo = maxAmmo;
            currentBottles = maxBottles;
        }
    }
}