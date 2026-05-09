using UnityEngine;

public class LevelSelectPanel : UIPanel
{
    [Header("Level Generation Settings")]
    [SerializeField] private LevelButton buttonPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private int totalLevels = 10;

    // Ghi đè hàm Open để mỗi khi mở Panel sẽ tự động cập nhật danh sách Level
    public override void Open()
    {
        base.Open();
        GenerateLevelButtons();
    }

    private void GenerateLevelButtons()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        for (int i = 1; i <= totalLevels; i++)
        {
            LevelButton newButton = Instantiate(buttonPrefab, gridParent);
            
            bool isUnlocked = (i < reachedLevel); 
            
            newButton.SetUp(i, isUnlocked);
        }
    }

    public void OnBackButtonClick()
    {
        UIManager.Instance.OpenPanel(UIPanelType.MainMenu);
    }
}