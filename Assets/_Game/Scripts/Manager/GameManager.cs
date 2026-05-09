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
        if (savedLevel > 10) savedLevel = 1;
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
        if (PlayerMovement.Instance != null) 
        {
            PlayerMovement.Instance.transform.position = Vector3.up * 100;
        }
        if (PlayerStack.Instance != null)
        {
            PlayerStack.Instance.ClearStack();
        }
        LevelManager.Instance.GenerateLevel();
        
        UIManager.Instance.OpenPanel(UIPanelType.GamePlay);
    }

    public void PreviousLevel()
    {
        Debug.Log(" đã back level");
        if (currentLevel > 1)
        {
            currentLevel = currentLevel - 1;
            RestartLevel();
        }
    }

    private IEnumerator HandleLoadingScreen()
    {
        UIManager.Instance.OpenPanel(UIPanelType.Loading);
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
        UIManager.Instance.OpenPanel(UIPanelType.MainMenu);
        Debug.Log("bật menu");

    }
}
