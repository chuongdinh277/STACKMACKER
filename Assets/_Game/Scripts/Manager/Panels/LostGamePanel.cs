using UnityEngine;
using TMPro;
public class LostGamePanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI levelText;
    public override void Open()
    {
        base.Open();

        if (GameManager.Instance != null && levelText != null)
        {
            levelText.text = "LEVEL " + GameManager.Instance.currentLevel;
        }
    }
    public void OnHomeButtonClick()
    {
        UIManager.Instance.OpenPanel(UIPanelType.MainMenu);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.score = 0;
        }

        if (PlayerStack.Instance != null)
        {
            PlayerStack.Instance.ClearStack();
        }
    }

    public void OnRetryButtonClick()
    {
        

        UIManager.Instance.StartCoroutine(UIManager.Instance.CurtainTransition(() => {
        this.Close();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
        UIManager.Instance.OpenPanel(UIPanelType.GamePlay);
        }));
    }
}