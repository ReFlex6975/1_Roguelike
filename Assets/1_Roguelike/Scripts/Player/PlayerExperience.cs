using UnityEngine;
using Unity.Netcode;
using Steamworks;

public class PlayerExperience : NetworkBehaviour
{
    [SerializeField] private float xpModifier = 1f;

    public NetworkVariable<int> Xp            = new NetworkVariable<int>();
    public NetworkVariable<int> Level         = new NetworkVariable<int>();
    public NetworkVariable<int> XpToNextLevel = new NetworkVariable<int>();
    public NetworkVariable<ulong> SteamId     = new NetworkVariable<ulong>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Xp.Value            = 0;
            Level.Value         = 1;
            XpToNextLevel.Value = 10;
        }

        if (IsOwner)
        {
            SetSteamIdServerRpc(SteamClient.SteamId.Value);

            Level.OnValueChanged         += (_, val) => HUD.Instance.LevelText.SetText($"Level: {val}");
            Xp.OnValueChanged            += (_, val) => HUD.Instance.XpText.SetText($"XP: {val} / {XpToNextLevel.Value}");
            XpToNextLevel.OnValueChanged += (_, val) => HUD.Instance.XpText.SetText($"XP: {Xp.Value} / {val}");

            UpdateUI();
        }
    }

    [ServerRpc]
    private void SetSteamIdServerRpc(ulong steamId) => SteamId.Value = steamId;

    private void UpdateUI()
    {
        HUD.Instance.LevelText.SetText($"Level: {Level.Value}");
        HUD.Instance.XpText.SetText($"XP: {Xp.Value} / {XpToNextLevel.Value}");
    }

    [ServerRpc]
    public void AddXpServerRpc(int amount)
    {
        Xp.Value += Mathf.RoundToInt(amount * xpModifier);
        while (Xp.Value >= XpToNextLevel.Value)
            LevelUp();
    }

    private void LevelUp()
    {
        Xp.Value -= XpToNextLevel.Value;
        Level.Value += 1;
        XpToNextLevel.Value += Level.Value * 2;

        Debug.Log($"[PlayerExperience] Игрок {OwnerClientId} достиг уровня {Level.Value}");
        // TODO: событие выбора апгрейда
    }
}