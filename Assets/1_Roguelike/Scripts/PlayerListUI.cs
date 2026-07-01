using System.Text;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

public class PlayerListUI : MonoBehaviour
{
    [SerializeField] private float refreshInterval = 1f;

    private float _timer;

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer > 0) return;

        _timer = refreshInterval;
        Refresh();
    }

    private void Refresh()
    {
        if (HUD.Instance.PlayerListText == null) return;
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) return;

        var sb = new StringBuilder();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;
            if (playerObj == null) continue;

            var exp = playerObj.GetComponent<PlayerExperience>();
            if (exp == null) continue;

            var steamId = exp.SteamId.Value;
            var name    = steamId != 0 ? new Friend(steamId).Name : $"Player {client.ClientId}";
            var level   = exp.Level.Value;

            sb.AppendLine($"{name} — Lvl {level}");
        }

        HUD.Instance.PlayerListText.SetText(sb.ToString());
    }
}
