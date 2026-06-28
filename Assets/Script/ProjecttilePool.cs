using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {

    [SerializeField]
    int poolSize;

    [SerializeField]
    GameObject projectilePrefab;

    List<Projectile> projectilesInPool;

    public static ProjectilePool Instance;

    void Awake () {
        Instance = GetComponent<ProjectilePool>();
    }

    void Start () {
        InitializePool();
    }

    void InitializePool () {
        projectilesInPool = new List<Projectile>();

        for (int i = 0; i < poolSize; i++) {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            // THE FIX: Make sure bullets start hidden!
            projectile.SetActive(false); 
            projectilesInPool.Add(projectile.GetComponent<Projectile>());
        }
    }

    public Projectile Instantiate(Vector3 position, Quaternion rotation, ulong shooterId)
    {
        Projectile projectile = projectilesInPool[0];
        projectile.transform.position = position;
        projectile.transform.rotation = rotation;

        // THE FIX: Tell the bullet who shot it BEFORE it wakes up!
        projectile.shooterId = shooterId;

        // Now turn it on!
        projectile.gameObject.SetActive(true); 

        projectilesInPool.Remove(projectile);
        return projectile;
    }

    public void ReturnToPool(Projectile projectile)
    {
        // THE FIX: Turn the bullet OFF when it returns to the pool
        projectile.gameObject.SetActive(false); 
        
        projectile.transform.position = transform.position;
        projectilesInPool.Add(projectile);
    }
}