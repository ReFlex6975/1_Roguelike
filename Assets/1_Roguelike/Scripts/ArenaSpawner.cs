using Unity.Netcode;
using UnityEngine;

public class ArenaSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float size = 20f;
    [SerializeField] private float wallHeight = 3f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        SpawnWallsClientRpc(size, wallHeight);
    }

    [ClientRpc]
    private void SpawnWallsClientRpc(float s, float h)
    {
        float hs = s / 2f;
        CreateWall(new Vector3(0,    h / 2f,  hs), new Vector3(s,  h, 1f)); // север
        CreateWall(new Vector3(0,    h / 2f, -hs), new Vector3(s,  h, 1f)); // юг
        CreateWall(new Vector3( hs,  h / 2f,   0), new Vector3(1f, h, s));  // восток
        CreateWall(new Vector3(-hs,  h / 2f,   0), new Vector3(1f, h, s));  // запад
    }

    private void CreateWall(Vector3 pos, Vector3 scale)
    {
        var wall = Instantiate(wallPrefab, pos, Quaternion.identity);
        wall.transform.localScale = scale;
    }
}
