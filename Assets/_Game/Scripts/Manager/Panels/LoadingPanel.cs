using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : UIPanel
{
    [SerializeField] private Image loadingFill;

    // Hàm này để cập nhật thanh progress từ GameManager
    public void UpdateProgress(float progress)
    {
        if (loadingFill != null)
        {
            loadingFill.fillAmount = progress;
        }
    }

    public override void Open()
    {
        base.Open();
        if (loadingFill != null) loadingFill.fillAmount = 0;
    }
}