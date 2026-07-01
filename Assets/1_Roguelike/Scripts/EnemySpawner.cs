using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 8f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnY = 1f;

    private float _timer;

    void Update()
    {
        if (!IsServer) return;

        _timer -= Time.deltaTime;
        if (_timer > 0) return;

        _timer = spawnInterval;
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        var offset = Random.insideUnitCircle * spawnRadius;
        var pos = new Vector3(offset.x, spawnY, offset.y);

        var go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
