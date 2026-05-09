using UnityEngine;
using TMPro;

public class GamePlayPanel : UIPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;

    public override void Open()
    {
        base.Open();
        UpdateLevelDisplay();
    }

    public void UpdateLevelDisplay()
    {
        if (levelText != null && GameManager.Instance != null)
        {
            levelText.text = "Level " + GameManager.Instance.currentLevel;
        }
    }

    public void OnSettingButtonClick()
    {
        UIManager.Instance.OpenPanel(UIPanelType.Settings);
    }
}