using UnityEngine;

public class WinPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement.Instance.TriggerWinEffects();
        }
    }
}
