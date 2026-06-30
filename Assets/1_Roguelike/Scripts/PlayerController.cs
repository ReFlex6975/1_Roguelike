using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    void Update()
    {
        if (!IsOwner) return;

        var horizontal = Input.GetAxis("Horizontal");
        var vertical   = Input.GetAxis("Vertical");

        if (horizontal == 0 && vertical == 0) return;

        var cam     = Camera.main;
        var forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        var right   = Vector3.ProjectOnPlane(cam.transform.right,   Vector3.up).normalized;

        var direction = (forward * vertical + right * horizontal).normalized;

        MoveServerRpc(direction);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 direction)
    {
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation  = Quaternion.LookRotation(direction);
    }
}
