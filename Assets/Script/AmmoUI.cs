using UnityEngine;
using TMPro; 
using Unity.Netcode;

public class AmmoUI : NetworkBehaviour {

    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private PlayerGun _myGun;
    [SerializeField] private GameObject _hudCanvas; 
    [SerializeField] private Player _myPlayer; 

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            if (_hudCanvas != null) _hudCanvas.SetActive(false);
        }
    }

    void Update() {
        if (!IsOwner) return;

        if (_myGun != null && _ammoText != null && _myPlayer != null) {
            _ammoText.text = "Ammo: " + _myGun.currentAmmo + " / 20\nBottles: " + _myGun.currentBottles + "\nInjectors: " + _myPlayer.injectors.Value;
        }
    }
}