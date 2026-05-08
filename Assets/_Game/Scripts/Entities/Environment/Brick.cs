using UnityEngine;

public class Brick : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("tìm được player");
            PlayerStack.Instance.PickUpBrick(this.gameObject);
        }
    }
}
