using UnityEngine;

public class LevelSelectMennu : MonoBehaviour
{
    [Header("Level Menu")]
    public LevelButton buttonPrefab;
    public Transform gridParent;
    public int totalLevels = 10;

    private void OnEnable()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        for (int i = 1; i <= totalLevels; i++ )
        {
            LevelButton newButton = Instantiate(buttonPrefab, gridParent);
            bool isUnlooked = false;
            if (i < reachedLevel) isUnlooked = true;
            newButton.SetUp(i, isUnlooked);
        }
    }
}
