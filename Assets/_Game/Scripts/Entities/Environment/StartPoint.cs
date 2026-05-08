using UnityEngine;

public class StartPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("startPoint đã tìm thấy player");
            PlayerStack.Instance.PickUpBrick(this.gameObject);
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
}
