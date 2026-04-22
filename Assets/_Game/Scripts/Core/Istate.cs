using UnityEngine;

public interface Istate
{
    public void OnEnter();
    public void OnExecute();
    
    public void OnExit();
}
