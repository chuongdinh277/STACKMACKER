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
        if (anim != null) anim.SetInteger("renwu", 2);
    }
    public void OnExecute()
    {
        
    }
    
    public void OnExit()
    {
        
    }
}
