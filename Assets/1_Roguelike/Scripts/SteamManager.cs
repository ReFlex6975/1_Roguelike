using System;
using Steamworks;
using UnityEngine;

// Инициализирует Steam до загрузки первой сцены.
// Steam overlay (Shift+Tab) должен зацепиться за DirectX до первого кадра —
// если инициализировать позже (при нажатии "Start Host"), overlay не внедряется.
public class SteamManager : MonoBehaviour
{
    private const uint AppId = 480;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        // Exclusive fullscreen блокирует Steam overlay — принудительно borderless
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        if (SteamClient.IsValid) return;

        try
        {
            SteamClient.Init(AppId, false);
            // Relay нужно инициализировать ДО ConnectRelay() — иначе клиент
            // падает при join, потому что FacepunchTransport делает это слишком поздно
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

    void OnApplicationQuit()
    {
        if (SteamClient.IsValid)
            SteamClient.Shutdown();
    }
}
