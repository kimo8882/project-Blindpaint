using UnityEngine;

public class PlayerHealthTest : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerHealth.TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth.Heal(10f);
        }
    }
}