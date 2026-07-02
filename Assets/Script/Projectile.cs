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
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;

        Vector3 direction = transform.position - _previousPosition;
        float distanceThisFrame = direction.magnitude;

        if (Physics.Raycast(_previousPosition, direction.normalized, out RaycastHit hit, distanceThisFrame))
        {

            Player enemyPlayer = hit.collider.GetComponent<Player>();

            if (enemyPlayer != null)
            {
                if (enemyPlayer.OwnerClientId != this.shooterId)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        enemyPlayer.TakeDamageServerRpc(1, this.shooterId);
                    }
                    ProjectilePool.Instance.ReturnToPool(this);
                }
            }
            else if (!hit.collider.isTrigger)
            {
                ProjectilePool.Instance.ReturnToPool(this);
            }
        }

        _previousPosition = transform.position;

        if (Vector3.Distance(_spawnPosition, transform.position) >= maxProjectileDistance)
        {
            ProjectilePool.Instance.ReturnToPool(this);
        }
    }
}