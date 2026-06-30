using Unity.Netcode;
using UnityEngine;

public class PlayerAutoAttack : NetworkBehaviour
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject projectilePrefab;

    private float _timer;

    void Update()
    {
        if (!IsOwner) return;

        _timer -= Time.deltaTime;
        if (_timer > 0) return;

        Collider nearest = FindNearest();
        if (nearest == null) return;

        _timer = cooldown;
        ShootServerRpc(nearest.GetComponent<NetworkObject>().NetworkObjectId);
    }

    private Collider FindNearest()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        Collider nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit;
            }
        }

        return nearest;
    }

    [ServerRpc]
    private void ShootServerRpc(ulong targetId)
    {
        var go = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        go.GetComponent<Projectile>().Init(targetId, damage);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
