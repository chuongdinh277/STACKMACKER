using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    [SerializeField] private MainMenuPanel mainMenuPanel;
    [SerializeField] private GamePlayPanel gamePlayPanel;
    [SerializeField] private SettingMenuPanel settingPanel; 
    [SerializeField] private LoadingPanel loadingPanel;
    [SerializeField] private WinPanel winGamePanel;
    [SerializeField] private LostGamePanel lostGamePanel;
    [SerializeField] private LevelSelectPanel levelSelectPanel;

    private List<UIPanel> allPanels = new List<UIPanel>();

    [Header("Shared Elements")]
    public TextMeshProUGUI currentLevelText;
    public TextMeshProUGUI currentMenuLevelText;
    public Image loadingFill;

    [Header("Transition Settings")]
    [SerializeField] private CanvasGroup transitionGroup;
    [SerializeField] private RectTransform leftCurtain;
    [SerializeField] private RectTransform rightCurtain;
    [SerializeField] private float transitionTime = 1.3f;
    private float offScreenPos = 540f;
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);   
            return;
        }
        allPanels.Add(mainMenuPanel);
        allPanels.Add(gamePlayPanel);
        allPanels.Add(settingPanel);
        allPanels.Add(loadingPanel);
        allPanels.Add(winGamePanel);
        allPanels.Add(lostGamePanel);
        allPanels.Add(levelSelectPanel);
    }

    public void OpenPanel(UIPanelType type)
    {
        if (type == UIPanelType.Settings && (winGamePanel.gameObject.activeSelf || lostGamePanel.gameObject.activeSelf))
        {
            return; 
        }
        foreach (var panel in allPanels)
        {
            if (panel != null) panel.Close();
        }

        switch (type)
        {
            case UIPanelType.MainMenu: mainMenuPanel.Open(); break;
            case UIPanelType.GamePlay: gamePlayPanel.Open(); break;
            case UIPanelType.Loading: loadingPanel.Open(); break;
            case UIPanelType.Win: winGamePanel.Open(); break;
            case UIPanelType.Lost: lostGamePanel.Open(); break;
            case UIPanelType.LevelSelect: levelSelectPanel.Open(); break;
            case UIPanelType.Settings: settingPanel.Open();break;
        }
        if (AudioManager.Instance != null)
        {
            if (type == UIPanelType.GamePlay)
            {
                AudioManager.Instance.StopMusic();
            }
            else
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.waitingMusic);
            }
        }
    }

    public void UpdateCurrentLevel(int level)
    {
        if (currentLevelText != null)
        {
            currentLevelText.text = "Level " + level.ToString();
        }

        if (currentMenuLevelText != null)
        {
            currentMenuLevelText.text = "level " + level.ToString();
        }
    }

    public void UpdateLoadingBar(float progress)
    {
        if (loadingPanel != null)
        {
            loadingPanel.UpdateProgress(progress);
        }
    }

    public void CloseGameplayPanel()
    {
        if (gamePlayPanel != null) gamePlayPanel.Close();
    }

    public IEnumerator CurtainTransition(System.Action onMidWay)
    {
        transitionGroup.blocksRaycasts = true;
        float elapsed = 0;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionTime);

            leftCurtain.anchoredPosition = new Vector2(Mathf.Lerp(-offScreenPos, 0, t), 0);
            rightCurtain.anchoredPosition = new Vector2(Mathf.Lerp(0, -offScreenPos, t), 0);
            yield return null;
        }

        onMidWay?.Invoke();
        yield return new WaitForSeconds(0.2f);

        elapsed = 0;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / transitionTime);

            leftCurtain.anchoredPosition = new Vector2(Mathf.Lerp(0, -offScreenPos, t), 0);
            rightCurtain.anchoredPosition = new Vector2(Mathf.Lerp(-offScreenPos, 0, t), 0);
            yield return null;
        }

        transitionGroup.blocksRaycasts = false;
    }
    public void OnPlayButtonClick() => OpenPanel(UIPanelType.GamePlay);
    public void OnLevelSelectButtonClick() => OpenPanel(UIPanelType.LevelSelect);
    public void OnSettingButtonClick() => settingPanel.Open();
    public void OnCloseSettingClick() => settingPanel.Close();
}
