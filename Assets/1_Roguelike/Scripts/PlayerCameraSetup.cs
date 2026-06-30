using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
public class PlayerCameraSetup : NetworkBehaviour
{
    [Header("Настройки камеры")]
    [SerializeField] private GameObject freeLookCameraPrefab; // Теперь это префаб с CinemachineCamera
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minY = -30f;
    [SerializeField] private float maxY = 60f;
    
    private CinemachineCamera _cinemachineCamera; // Изменено: теперь это CinemachineCamera
    private GameObject _cameraInstance;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        
        CreateCamera();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (_cameraInstance != null)
        {
            Destroy(_cameraInstance);
            _cameraInstance = null;
        }
    }

    private void CreateCamera()
    {
        if (freeLookCameraPrefab == null)
        {
            Debug.LogError("[Camera] FreeLook Camera Prefab не назначен!");
            return;
        }

        // Проверяем, что префаб содержит правильный компонент (теперь CinemachineCamera)
        var prefabCamera = freeLookCameraPrefab.GetComponent<CinemachineCamera>();
        if (prefabCamera == null)
        {
            Debug.LogError($"[Camera] Префаб '{freeLookCameraPrefab.name}' не содержит CinemachineCamera! Убедитесь, что вы создали правильную камеру.");
            return;
        }

        // Создаем экземпляр камеры
        _cameraInstance = Instantiate(freeLookCameraPrefab, transform.position, Quaternion.identity);
        _cinemachineCamera = _cameraInstance.GetComponent<CinemachineCamera>();
        
        if (_cinemachineCamera == null)
        {
            Debug.LogError("[Camera] Не удалось получить компонент CinemachineCamera!");
            return;
        }

        // Настраиваем слежение (Follow и LookAt)
        _cinemachineCamera.Follow = transform;
        _cinemachineCamera.LookAt = transform;
        
        // Настройка управления (через Input Axis Controller)
        // Вам нужно найти компонент CinemachineInputAxisController на камере
        var axisController = _cameraInstance.GetComponent<CinemachineInputAxisController>();
        if (axisController != null)
        {
            // Настройка чувствительности для осей X и Y
            // Названия осей могут отличаться, обычно "Mouse X" и "Mouse Y"
            // Настройка может быть сложнее, но для начала можно оставить стандартные параметры
        }
        
        // Устанавливаем приоритет (высокий для владельца)
        _cinemachineCamera.Priority = 100;
        
        Debug.Log($"[Camera] Новая FreeLook-камера (CinemachineCamera) создана для игрока: {OwnerClientId}");
    }
}