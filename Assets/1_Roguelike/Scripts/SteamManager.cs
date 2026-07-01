using System;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using Unity.Netcode;
using UnityEngine;

// DontDestroyOnLoad — колбэки Steam всегда активны независимо от состояния сцены.
public class SteamManager : MonoBehaviour
{
    private const uint AppId = 480;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        if (SteamClient.IsValid) return;

        try
        {
            SteamClient.Init(AppId, false);
            SteamNetworkingUtils.InitRelayNetworkAccess();
            Debug.Log($"[SteamManager] Инициализирован: {SteamClient.Name} ({SteamClient.SteamId})");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SteamManager] Ошибка инициализации Steam: {e.Message}");
            return;
        }

        var go = new GameObject("[SteamManager]");
        DontDestroyOnLoad(go);
        go.AddComponent<SteamManager>();
    }

    void Awake()
    {
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    void OnDestroy()
    {
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
    }

    // Клиент кликнул "Join Game" в оверлее — всё делаем здесь, без OnLobbyEntered
    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Debug.Log($"[SteamManager] OnGameLobbyJoinRequested: лобби={lobby.Id}");

        var nm = NetworkManager.Singleton;
        if (nm.IsHost || nm.IsClient)
        {
            Debug.Log("[SteamManager] Уже подключены, игнорируем");
            return;
        }

        var result = await lobby.Join();
        Debug.Log($"[SteamManager] lobby.Join() → {result}");

        if (result != RoomEnter.Success)
        {
            Debug.LogError($"[SteamManager] Не удалось войти в лобби: {result}");
            return;
        }

        // После Join() lobby.Owner гарантированно содержит данные хоста
        var transport = nm.GetComponent<FacepunchTransport>();
        if (transport == null)
        {
            Debug.LogError("[SteamManager] FacepunchTransport не найден на NetworkManager");
            return;
        }

        transport.targetSteamId = lobby.Owner.Id;
        Debug.Log($"[SteamManager] Подключаемся к {lobby.Owner.Name} ({lobby.Owner.Id})");
        nm.StartClient();
    }

    void OnApplicationQuit()
    {
        if (SteamClient.IsValid)
            SteamClient.Shutdown();
    }
}
