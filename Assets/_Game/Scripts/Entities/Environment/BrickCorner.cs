using System.Collections;
using UnityEngine;

public class BrickCorner : MonoBehaviour, Istate
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnEnter()
    {
        if (anim != null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayAndReset());
        }
    }

    private IEnumerator PlayAndReset()
    {
        anim.SetInteger("zhuanjiaoSet", 1);
        
        // 2. Đợi một khoảng thời gian (tùy độ dài hiệu ứng của ông, ví dụ 0.5s)
        yield return new WaitForSeconds(1f);
        
        // 3. Tắt hiệu ứng
        anim.SetInteger("zhuanjiaoSet", 0);
    }
    public void OnExecute()
    {
    }
    
    public void OnExit()
    {
    }
}
