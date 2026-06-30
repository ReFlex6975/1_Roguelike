using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    private FacepunchTransport _transport;
    private Lobby? _currentLobby;

    void Awake()
    {
        _transport = FindFirstObjectByType<FacepunchTransport>();
    }

    void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated       += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered       += OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated       -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered       -= OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
    }

    // ── кнопки UI ──────────────────────────────────────────────

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        CreateSteamLobby();
    }

    public void StartClient()
    {
        // ручное подключение (без Steam-оверлея) — для тестов в редакторе
        NetworkManager.Singleton.StartClient();
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
        _currentLobby.Value.SetFriendsOnly();   // видно друзьям через Shift+Tab
        _currentLobby.Value.SetJoinable(true);
        Debug.Log($"[Steam] Лобби создано: {_currentLobby.Value.Id}");
    }

    // ── Steam: колбэки ─────────────────────────────────────────

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
            Debug.LogError($"[Steam] OnLobbyCreated error: {result}");
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        if (NetworkManager.Singleton.IsHost) return;

        // клиент вошёл в лобби — получаем SteamId хоста и подключаемся
        _transport.targetSteamId = lobby.Owner.Id;
        NetworkManager.Singleton.StartClient();
        Debug.Log($"[Steam] Подключаемся к хосту: {lobby.Owner.Name} ({lobby.Owner.Id})");
    }

    // срабатывает, когда игрок кликает "Join Game" в Steam-оверлее (Shift+Tab)
    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        var result = await lobby.Join();
        if (result != RoomEnter.Success)
            Debug.LogError($"[Steam] Не удалось войти в лобби: {result}");
    }
}
