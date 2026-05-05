using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Button button;
    private int levelIndex;

    public void SetUp(int index, bool isUnlooked)
    {
        levelIndex = index;
        levelText.text = index.ToString();

        button.interactable = isUnlooked;
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        GameManager.Instance.currentLevel = levelIndex;
        LevelManager.Instance.GenerateLevel();
        UIManager.Instance.levelSelectPanel.Close();
        UIManager.Instance.OnPlayButtonClick();
    }
}
