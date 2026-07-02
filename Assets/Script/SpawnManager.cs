using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPoints;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetRandomSpawnPosition()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            Debug.LogWarning(" SPAWN MANAGER: No spawn points assigned in the Inspector! Defaulting to center.");
            return new Vector3(0, 2, 0);
        }

        int randomIndex = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[randomIndex].position;
    }
}