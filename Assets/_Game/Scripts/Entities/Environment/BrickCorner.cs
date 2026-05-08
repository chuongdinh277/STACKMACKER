using System.Collections;
using UnityEngine;

public class BrickCorner : MonoBehaviour, Istate
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

   private void OnTriggerEnter(Collider other) 
   {
    if (other.CompareTag("Player")) {
        Vector3 cornerPos = transform.position;
        // Gọi hiệu ứng xoay của Corner
        OnEnter(); 
        // Điều khiển Player rẽ hướng
        if (PlayerMovement.Instance.IsMoving) {
            PlayerMovement.Instance.Redirect(PlayerMovement.Instance.MoveVec, cornerPos);
        }
    }
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
        
        yield return new WaitForSeconds(1f);
        
        anim.SetInteger("zhuanjiaoSet", 0);
    }
    public void OnExecute()
    {
    }
    
    public void OnExit()
    {
    }
}
