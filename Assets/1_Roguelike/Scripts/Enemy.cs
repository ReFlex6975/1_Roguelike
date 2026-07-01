using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float maxHp = 50f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private GameObject XpCrystalPrefab;

    public NetworkVariable<float> Hp = new NetworkVariable<float>();

    private float _damageTimer;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            Hp.Value = maxHp;
    }

    void Update()
    {
        if (!IsServer) return;

        _damageTimer -= Time.deltaTime;

        Transform target = FindNearestPlayer();
        if (target == null) return;

        var dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerStay(Collider other)
    {
        if (!IsServer || _damageTimer > 0) return;
        if (!other.CompareTag("Player")) return;

        var combat = other.GetComponent<PlayerCombat>();
        if (combat == null) return;

        combat.TakeDamage(damage);
        _damageTimer = damageCooldown;
    }

    public void TakeDamage(float dmg)
    {
        if (!IsServer) return;

        Hp.Value -= dmg;
        if (Hp.Value <= 0)
        {
            var go = Instantiate(XpCrystalPrefab, transform.position, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
            NetworkObject.Despawn();
        }
    }

    private Transform FindNearestPlayer()
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var client in NetworkManager.ConnectedClients.Values)
        {
            if (client.PlayerObject == null) continue;
            float dist = Vector3.Distance(transform.position, client.PlayerObject.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = client.PlayerObject.transform;
            }
        }

        return nearest;
    }
}
