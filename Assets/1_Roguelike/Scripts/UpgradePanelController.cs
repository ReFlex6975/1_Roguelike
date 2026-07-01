using UnityEngine;

public class UpgradePanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    public void Toggle() => panel.SetActive(!panel.activeSelf);

    public void Close() => panel.SetActive(false);
}
