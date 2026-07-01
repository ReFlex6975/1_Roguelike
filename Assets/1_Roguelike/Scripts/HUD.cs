using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI XpText;
    public TextMeshProUGUI PlayerListText;

    [Header("Upgrade Panel")]
    public GameObject SkillPointsBadge;
    public TextMeshProUGUI SkillPointsBadgeText;
    public GameObject UpgradePanel;
    public TextMeshProUGUI UpgradePanelPointsText;

    void Awake() => Instance = this;

    public void SetSkillPoints(int points)
    {
        if (SkillPointsBadge != null)
            SkillPointsBadge.SetActive(points > 0);

        if (SkillPointsBadgeText != null)
            SkillPointsBadgeText.SetText($"SP: {points}");

        if (UpgradePanelPointsText != null)
            UpgradePanelPointsText.SetText($"Points: {points}");
    }
}