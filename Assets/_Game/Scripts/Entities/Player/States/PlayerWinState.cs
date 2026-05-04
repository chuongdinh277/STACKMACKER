using UnityEngine;

public class PlayerWinState : Istate
{
    private Animator anim;

    public PlayerWinState(Animator animator)
    {
        anim = animator;
    }
    public void OnEnter()
    {
        if (anim != null)
        {
            anim.SetInteger("renwu", (int)PlayerAnimState.Win);
            anim.Play("Take 3", 0, 0f); 
        }
    }
    public void OnExecute()
    {
        
    }
    
    public void OnExit()
    {
        
    }
}
