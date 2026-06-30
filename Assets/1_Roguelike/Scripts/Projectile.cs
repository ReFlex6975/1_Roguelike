using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private ulong _targetId;
    private float _speed = 10f;
    private float _damage = 10f;

    public void Init(ulong targetId, float damage)
    {
        _targetId = targetId;
        _damage = damage;
    }

    void Update()
    {
        if (!IsServer) return; // движение считает только хост

        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(_targetId, out var target))
        {
            var dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * _speed * Time.deltaTime;
        }
        else
        {
            // цель умерла — уничтожаем снаряд
            NetworkObject.Despawn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.CompareTag("Enemy")) return;

        other.GetComponent<Enemy>()?.TakeDamage(_damage);
        NetworkObject.Despawn();
    }
}
