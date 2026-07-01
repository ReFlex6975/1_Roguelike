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
        if (_transport == null)
            Debug.LogError("[Buttons] FacepunchTransport не найден на сцене!");
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
        _currentLobby.Value.SetFriendsOnly();
        _currentLobby.Value.SetJoinable(true);

        // КРИТИЧНО: без SetGameServer оверлей Shift+Tab не показывает "Join Game"
        _currentLobby.Value.SetGameServer(SteamClient.SteamId);

        Debug.Log($"[Steam] Лобби создано: {_currentLobby.Value.Id} | хост SteamId: {SteamClient.SteamId}");
    }

    // ── Steam: колбэки ─────────────────────────────────────────

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
            Debug.LogError($"[Steam] OnLobbyCreated ошибка: {result}");
        else
            Debug.Log("[Steam] OnLobbyCreated — OK");
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        var nm = NetworkManager.Singleton;
        Debug.Log($"[Steam] OnLobbyEntered: IsHost={nm.IsHost} IsClient={nm.IsClient} IsListening={nm.IsListening}");

        // хост и уже подключающийся клиент — пропускаем
        if (nm.IsHost || nm.IsClient) return;

        if (_transport == null)
        {
            Debug.LogError("[Steam] _transport == null, подключение невозможно");
            return;
        }

        _transport.targetSteamId = lobby.Owner.Id;
        Debug.Log($"[Steam] Подключаемся к {lobby.Owner.Name} (SteamId={lobby.Owner.Id})");
        nm.StartClient();
    }

    // срабатывает, когда клиент кликает "Join Game" в Steam-оверлее
    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Debug.Log($"[Steam] OnGameLobbyJoinRequested: лобби={lobby.Id} от={steamId}");
        var result = await lobby.Join();
        Debug.Log($"[Steam] lobby.Join() → {result}");

        if (result != RoomEnter.Success)
            Debug.LogError($"[Steam] Не удалось войти в лобби: {result}");
        // при успехе сработает OnLobbyEntered, там стартуем клиент
    }
}
