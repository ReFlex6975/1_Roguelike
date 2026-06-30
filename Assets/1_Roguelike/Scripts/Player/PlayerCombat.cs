using Unity.Netcode;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField] private float maxHp = 100f;

    public NetworkVariable<float> Hp = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            Hp.Value = maxHp;
    }

    // вызывается только с сервера (Enemy, Projectile)
    public void TakeDamage(float damage)
    {
        if (!IsServer) return;

        Hp.Value -= damage;
        Debug.Log($"[PlayerCombat] Client {OwnerClientId} получил {damage} урона. HP: {Hp.Value}");

        if (Hp.Value <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"[PlayerCombat] Client {OwnerClientId} умер");
        // TODO: обработка смерти (респаун, гейм овер)
        NetworkObject.Despawn();
    }
}
