using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    private Lobby? _currentLobby;

    void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
    }

    void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
    }

    // ── кнопки UI ──────────────────────────────────────────────

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        CreateSteamLobby();
    }

    // ── Steam: создание лобби ───────────────────────────────────

    private async void CreateSteamLobby()
    {
        var lobby = await SteamMatchmaking.CreateLobbyAsync(4);
        if (!lobby.HasValue)
        {
            Debug.LogError("[Steam] Не удалось создать лобби");
            return;
        }

        _currentLobby = lobby.Value;
        _currentLobby.Value.SetFriendsOnly();
        _currentLobby.Value.SetJoinable(true);
        _currentLobby.Value.SetGameServer(SteamClient.SteamId);

        Debug.Log($"[Steam] Лобби создано: {_currentLobby.Value.Id} | хост: {SteamClient.SteamId}");
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
            Debug.LogError($"[Steam] OnLobbyCreated ошибка: {result}");
        else
            Debug.Log("[Steam] OnLobbyCreated — OK");
    }
}
