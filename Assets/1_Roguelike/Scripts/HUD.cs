using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI XpText;
    public TextMeshProUGUI PlayerListText;

    void Awake() => Instance = this;
}