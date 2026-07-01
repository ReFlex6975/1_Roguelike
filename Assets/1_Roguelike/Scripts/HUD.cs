using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI XpText;

    void Awake() => Instance = this;
}