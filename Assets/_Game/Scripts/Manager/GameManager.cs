using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public bool isPlaying = false;
    public int currentLevel = 1;
    public int score = 0;
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


    // kích hoạt khi người chơi bấm nút start
    public void StartGame()
    {
        isPlaying = true;
    }


    public void RestartLevel()
    {
        Debug.Log(" đã restart level");

        Time.timeScale = 1;
        score = 0;

        FindObjectOfType<PlayerStack>().ClearStack();
        LevelManager.Instance.GenerateLevel();
        
        UIManager.Instance.OnCloseSettingClick();
    }

    public void PreviousLevel()
    {
        Debug.Log(" đã restart level");
        if (currentLevel > 1)
        {
            currentLevel = currentLevel - 1;
            Time.timeScale = 1;
            score = 0;
            FindObjectOfType<PlayerStack>().ClearStack();
            LevelManager.Instance.GenerateLevel();
            UIManager.Instance.OnCloseSettingClick();
        }
    }
}
