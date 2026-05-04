using UnityEngine;

public class PlayerMoveState : Istate
{
    private Animator anim;

    public PlayerMoveState(Animator animator)
    {
        anim = animator;
    }
    public void OnEnter()
    {
        if (anim != null)
        {
            anim.SetInteger("renwu",(int)PlayerAnimState.Run);
            anim.Play("Take 2", 0, 0f);
        }
    }
    public void OnExecute()
    {
        
    }
    public void OnExit()
    {
        
    }
}
