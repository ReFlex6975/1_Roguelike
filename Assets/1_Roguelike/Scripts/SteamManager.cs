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
        if (SteamClient.IsValid) return;

        try
        {
            SteamClient.Init(AppId, false);
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
