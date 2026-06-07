using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    // CRITICAL: Make sure this exact line is here at the top!
    [Header("Assign your Player Prefab here!")]
    public GameObject playerPrefab;

    void Start() { }

    public void SpawnPlayer()
    {
        GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (spawnLocations.Length == 0)
        {
            Debug.LogError("Uh oh! No SpawnPoints found.");
            return;
        }

        int randomIndex = Random.Range(0, spawnLocations.Length);
        Transform chosenCorner = spawnLocations[randomIndex].transform;

        Instantiate(playerPrefab, chosenCorner.position, chosenCorner.rotation);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME button was clicked!");

        Application.Quit();
    }
}
