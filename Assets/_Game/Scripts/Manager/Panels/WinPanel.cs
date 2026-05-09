using UnityEngine;
using TMPro;

public class WinPanel : UIPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI brickCountText;

    public override void Open()
    {
        base.Open();
        UpdateWinStats();
    }

    private void UpdateWinStats()
    {
        if (GameManager.Instance != null)
        {
            if (levelText != null) 
                levelText.text = "LEVEL " + GameManager.Instance.currentLevel;

            if (brickCountText != null)
                brickCountText.text = GameManager.Instance.score.ToString();
        }
    }

    public void OnRetryButtonClick()
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

    public void OnNextLevelButtonClick()
    {
        this.Close(); 

        UIManager.Instance.StartCoroutine(UIManager.Instance.CurtainTransition(() => {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.NextLevel();
        }
        UIManager.Instance.OpenPanel(UIPanelType.GamePlay);
        }));
    }
}