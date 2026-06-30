using Unity.Netcode;
using UnityEngine;

public class DebugEnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 8f;

    void Update()
    {
        if (!IsServer) return;
        if (!Input.GetKeyDown(KeyCode.F1)) return;

        var pos = Random.insideUnitSphere * spawnRadius;
        pos.y = 0;

        var go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
