using UnityEngine;
using TMPro;

public class SettingMenuPanel : UIPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI brickCountText;

    public override void Open()
    {
        base.Open();
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        if (GameManager.Instance != null)
        {
            if (levelText != null) 
                levelText.text = "LEVEL " + GameManager.Instance.currentLevel;

            if (brickCountText != null)
                brickCountText.text = GameManager.Instance.score.ToString();
        }
    }

    public void OnCloseButtonClick()
    {
        UIManager.Instance.OpenPanel(UIPanelType.GamePlay);
    }

    public void OnBackLevelButtonClick()
    {
        this.Close(); 
        
        UIManager.Instance.StartCoroutine(UIManager.Instance.CurtainTransition(() => {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PreviousLevel(); 
                
                UpdateInfo();
            }
            UIManager.Instance.OpenPanel(UIPanelType.GamePlay);
        }));
    }

    public void OnRestartButtonClick()
    {
        this.Close();
        UIManager.Instance.StartCoroutine(UIManager.Instance.CurtainTransition(() => {
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel(); 
        }
        UIManager.Instance.OpenPanel(UIPanelType.GamePlay);
        }));
    }
}