using System.Collections;
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

    public void Start()
    {
        int savedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        currentLevel = savedLevel;
        Debug.Log(currentLevel);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCurrentLevel(currentLevel);
        }
        StartCoroutine(HandleLoadingScreen());
    }
    public void StartGame()
    {
        isPlaying = true;
    }


    public void RestartLevel()
    {
        Debug.Log(" đã restart level");

        Time.timeScale = 1;
        score = 0;
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null) pm.transform.position = Vector3.up * 100;
        FindObjectOfType<PlayerStack>().ClearStack();
        LevelManager.Instance.GenerateLevel();
        
        UIManager.Instance.OnCloseSettingClick();
    }

    public void PreviousLevel()
    {
        Debug.Log(" đã back level");
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

    private IEnumerator HandleLoadingScreen()
    {
        UIManager.Instance.mainMenuPanel.Close();
        UIManager.Instance.gamePlayPanel.Close();
        float duration = 1.5f;
        float elapse = 0f;

        while (elapse < duration)
        {
            elapse = elapse + Time.deltaTime;
            float progess = elapse / duration;
            UIManager.Instance.UpdateLoadingBar(progess);
            yield return null;
        }
        LevelManager.Instance.GenerateLevel();
        yield return new WaitForEndOfFrame();
        UIManager.Instance.loadingPanel.Close();
        UIManager.Instance.mainMenuPanel.Open();
        Debug.Log("bật menu");
        //UIManager.Instance.gamePlayPanel.SetActive(true);

        
    }
}
