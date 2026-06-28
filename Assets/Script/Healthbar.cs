using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Image healthbarSprite;

    private Camera cam;

    void Start() {
        cam = Camera.main;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth) {
        if (healthbarSprite != null)
        {
            healthbarSprite.fillAmount = currentHealth / maxHealth;
        }
    }

    void Update() {
        // FIXED FOR TOP-DOWN: Instead of calculating a relative position vector,
        // we force the canvas to match the exact view plane tilt of the camera lens.
        transform.rotation = cam.transform.rotation;
    }
}