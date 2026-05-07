using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public UIPanel mainMenuPanel;
    public UIPanel gamePlayPanel;
    public UIPanel settingPanel;
    public UIPanel loadingPanel;
    public UIPanel winGamePanel;
    public UIPanel lostGamePanel;
    public UIPanel levelSelectPanel;


    [Header("Gameplay Elements")]
    public TextMeshProUGUI currentLevelText;

    [Header("Setting Elements")]
    public TextMeshProUGUI levelTextSetting;
    public TextMeshProUGUI brickCountText;


    [Header("Loading Elements")]
   
    public Image loadingFill;


    [Header("Win Panel Elements")]
    public TextMeshProUGUI winLevelText;
    public TextMeshProUGUI winBrickCountText;

    [Header("Lost Panel Elements")]
    public TextMeshProUGUI lostLevelText;
    [Header("Gameplay Elements")]
    public TextMeshProUGUI currentMenuLevelText;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayButtonClick()
    {
        mainMenuPanel.Close();
        gamePlayPanel.Open();
        GameManager.Instance.StartGame();
    }

    public void OnSettingButtonClick()
    {
        Debug.Log("đã bấm button");
        settingPanel.Open();
        Time.timeScale = 0;

        levelTextSetting.text = "LEVEL" + GameManager.Instance.currentLevel;
        brickCountText.text = GameManager.Instance.score.ToString();
    }

    public void OnCloseSettingClick()
    {
        settingPanel.Close();
        Time.timeScale = 1;
    }

    public void ShowWinPanel()
    {
        
        if (winBrickCountText != null)
        {
            winBrickCountText.text = GameManager.Instance.score.ToString();
        }

        if (winLevelText != null)
        {
            winLevelText.text = "LEVEL " + GameManager.Instance.currentLevel;
        }
        winGamePanel.Open();
    }

    public void ShowLostPanel()
    {
        if (lostLevelText != null)
        {
            lostLevelText.text = "LEVEL " + GameManager.Instance.currentLevel;
        }
        lostGamePanel.Open();
    }

    public void OnNextLevelClick()
    {
        winGamePanel.Close();
        gamePlayPanel.Open();
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.NextLevel();
        }
    }

    public void OnRetryLevelClick()
    {
        winGamePanel.Close();
        lostGamePanel.Close();
        gamePlayPanel.Open();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
    } 
    public void UpdateCurrentLevel(int level)
    {
        currentLevelText.text = "LEVEL " + level;
        currentMenuLevelText.text = "LEVEL " + level;
    }

    public void UpdateLoadingBar(float progess)
    {
        if (loadingFill != null)
        {
            loadingFill.fillAmount = progess;
        }
    }

    public void OnLevelSelectButtonClick()
    {
        mainMenuPanel.Close();
        levelSelectPanel.Open();
    }

    public void OnCloseLevelSelectClick()
    {
        levelSelectPanel.Close();
        mainMenuPanel.Open();
    }

    public void OnCloseGamePlayPanel()
    {
        if (gamePlayPanel != null)
        {
            gamePlayPanel.Close();
        }
    }
}
