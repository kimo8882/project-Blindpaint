using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Settings")]
    public float projectileSpeed = 25f;
    public float maxProjectileDistance = 15f;
    public ulong shooterId;

    private Vector3 _spawnPosition;
    private Vector3 _previousPosition;

    void OnEnable()
    {
        _spawnPosition = transform.position;
        _previousPosition = transform.position;
    }

    void Update()
    {
        // 1. Move the visual bullet forward
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;

        // 2. The AAA Anti-Tunneling Laser
        Vector3 direction = transform.position - _previousPosition;
        float distanceThisFrame = direction.magnitude;

        // Shoot a laser from exactly where we were last frame to where we are now
        if (Physics.Raycast(_previousPosition, direction.normalized, out RaycastHit hit, distanceThisFrame))
        {

            // Did the laser hit a player?
            Player enemyPlayer = hit.collider.GetComponent<Player>();

            if (enemyPlayer != null)
            {
                // If it is an enemy, deal damage!
                if (enemyPlayer.OwnerClientId != this.shooterId)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        enemyPlayer.TakeDamageServerRpc(1, this.shooterId);
                    }
                    ProjectilePool.Instance.ReturnToPool(this);
                }
            }
            // Did the laser hit a wall/obstacle? (Anything that isn't a trigger zone)
            else if (!hit.collider.isTrigger)
            {
                ProjectilePool.Instance.ReturnToPool(this);
            }
        }

        // 3. Update previous position for the next frame's math
        _previousPosition = transform.position;

        // 4. Destroy if it flies too far into the sky
        if (Vector3.Distance(_spawnPosition, transform.position) >= maxProjectileDistance)
        {
            ProjectilePool.Instance.ReturnToPool(this);
        }
    }
}