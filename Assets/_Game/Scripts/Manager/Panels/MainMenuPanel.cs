using UnityEngine;

public class MainMenuPanel : UIPanel
{
    public void OnPlayButtonClick()
    {
        
        UIManager.Instance.StartCoroutine(UIManager.Instance.CurtainTransition(() => {
        this.Close();
        UIManager.Instance.OpenPanel(UIPanelType.GamePlay);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        }));
    }

    public void OnLevelSelectButtonClick()
    {
        UIManager.Instance.OpenPanel(UIPanelType.LevelSelect);
    }

    public void OnSettingButtonClick()
    {
        UIManager.Instance.OpenPanel(UIPanelType.Settings);
    }
}