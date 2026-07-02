using UnityEngine;

public class DamageScreenEffect : MonoBehaviour
{
    [Header("Assign your 3 Splash UI Objects here")]
    public GameObject[] splashImages;

    public void UpdateDamageScreen(int currentHealth, int maxHealth)
    {
        foreach (GameObject splash in splashImages)
        {
            if (splash != null) splash.SetActive(false);
        }

        int missingHealth = maxHealth - currentHealth;

        if (missingHealth >= 1 && splashImages.Length > 0) splashImages[0].SetActive(true);
        if (missingHealth >= 2 && splashImages.Length > 1) splashImages[1].SetActive(true);
        if (missingHealth >= 3 && splashImages.Length > 2) splashImages[2].SetActive(true);
    }
}