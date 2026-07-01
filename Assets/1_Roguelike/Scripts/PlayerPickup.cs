using Unity.Netcode;
using UnityEngine;

public class PlayerPickup : NetworkBehaviour
{
    private PlayerExperience _experience;

    void Awake()
    {
        _experience = GetComponent<PlayerExperience>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if (!other.CompareTag("Xp")) return;

        var pickup = other.GetComponent<XpPickup>();
        if (pickup == null) return;

        _experience.AddXpServerRpc(pickup.Amount);
        Destroy(other.gameObject);
    }
}
