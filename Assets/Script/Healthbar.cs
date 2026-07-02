using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] private Image healthbarSprite;

    [Header("Positioning Settings")]
    [Tooltip("How high above the player's feet should the health bar hover?")]
    [SerializeField] private Vector3 _headOffset = new Vector3(0, 2.2f, 0);

    [Tooltip("How far towards the camera should we pull the UI to avoid walls?")]
    [SerializeField] private float _cameraPullDistance = 1.5f;

    private Camera cam;
    private Transform _playerTransform;

    void Start()
    {
        cam = Camera.main;

        Player playerScript = GetComponentInParent<Player>();
        if (playerScript != null)
        {
            _playerTransform = playerScript.transform;
        }

        if (healthbarSprite == null)
        {
            Image[] allImages = GetComponentsInChildren<Image>();
            foreach (Image img in allImages)
            {
                if (img.gameObject.name == "Foreground")
                {
                    healthbarSprite = img;
                    break;
                }
            }
        }

        if (healthbarSprite == null)
        {
            Debug.LogError("HEALTHBAR: I searched everywhere but I cannot find an object named 'Foreground'!");
        }
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        if (healthbarSprite != null)
        {
            healthbarSprite.fillAmount = currentHealth / maxHealth;
        }
    }

    void Update()
    {
        if (cam != null && _playerTransform != null)
        {
            transform.rotation = cam.transform.rotation;

            Vector3 defaultPosition = _playerTransform.position + _headOffset;

            Vector3 directionToCamera = (cam.transform.position - defaultPosition).normalized;

            transform.position = defaultPosition + (directionToCamera * _cameraPullDistance);
        }
    }
}