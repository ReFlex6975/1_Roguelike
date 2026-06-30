using Unity.Netcode;
using UnityEngine;

public class PlayerCameraSetup : NetworkBehaviour
{
    [SerializeField] private float distance = 5f;
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float heightOffset = 1.5f;

    private Camera _camera;
    private float _yaw;
    private float _pitch = 20f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        var go = new GameObject("PlayerCamera");
        go.tag = "MainCamera";
        _camera = go.AddComponent<Camera>();
        go.AddComponent<AudioListener>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnNetworkDespawn()
    {
        if (_camera != null)
            Destroy(_camera.gameObject);
    }

    void LateUpdate()
    {
        if (!IsOwner || _camera == null) return;

        _yaw   += Input.GetAxis("Mouse X") * sensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        _pitch  = Mathf.Clamp(_pitch, minPitch, maxPitch);

        var pivot    = transform.position + Vector3.up * heightOffset;
        var rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        _camera.transform.position = pivot + rotation * new Vector3(0, 0, -distance);
        _camera.transform.LookAt(pivot);
    }
}
