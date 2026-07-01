using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerExperience : NetworkBehaviour
{
    [SerializeField] private float xpModifier = 1f;

    public NetworkVariable<int> Xp = new NetworkVariable<int>();
    public NetworkVariable<int> Level = new NetworkVariable<int>();
    public NetworkVariable<int> XpToNextLevel = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Xp.Value = 0;
            Level.Value = 1;
            XpToNextLevel.Value = 10;
        }

        if (IsOwner)
        {
            Level.OnValueChanged += (_, val) => HUD.Instance.LevelText.SetText($"Level: {val}");
            Xp.OnValueChanged += (_, val) => HUD.Instance.XpText.SetText($"XP: {val} / {XpToNextLevel.Value}");
            XpToNextLevel.OnValueChanged += (_, val) => HUD.Instance.XpText.SetText($"XP: {Xp.Value} / {val}");

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        HUD.Instance.LevelText.SetText($"Level: {Level.Value}");
        HUD.Instance.XpText.SetText($"XP: {Xp.Value} / {XpToNextLevel.Value}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if (!other.CompareTag("Xp")) return;

        var pickup = other.GetComponent<XpPickup>();
        if (pickup == null) return;

        AddXpServerRpc(pickup.Amount, other.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ServerRpc]
    private void AddXpServerRpc(int amount, ulong pickupNetId)
    {
        Xp.Value += Mathf.RoundToInt(amount * xpModifier);

        while (Xp.Value >= XpToNextLevel.Value)
            LevelUp();

        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(pickupNetId, out var netObj))
            netObj.Despawn();
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