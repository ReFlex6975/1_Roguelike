using Steamworks;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    void Awake()
    {
        try
        {
            SteamClient.Init(480);
            Debug.Log($"[Steam] Инициализирован как: {SteamClient.Name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Steam] Ошибка инициализации: {e.Message}");
        }
    }

    void OnDestroy()
    {
        SteamClient.Shutdown();
    }
}
