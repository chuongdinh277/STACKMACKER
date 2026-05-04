using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePlayPanel;
    public GameObject settingPanel;


    [Header("Gameplay Elements")]
    public TextMeshProUGUI currentLevelText;

    [Header("Setting Elements")]
    public TextMeshProUGUI levelTextSetting;
    public TextMeshProUGUI brickCountText;

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
        mainMenuPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
        GameManager.Instance.StartGame();
    }

    public void OnSettingButtonClick()
    {
        Debug.Log("đã bấm button");
        settingPanel.SetActive(true);
        Time.timeScale = 0;

        levelTextSetting.text = "LEVEL" + GameManager.Instance.currentLevel;
        brickCountText.text = GameManager.Instance.score.ToString();
    }

    public void OnCloseSettingClick()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ToggleSound()
    {
        
    }

    public void UpdateCurrentLevel(int level)
    {
        currentLevelText.text = "LEVEL" + level;
    }
}
