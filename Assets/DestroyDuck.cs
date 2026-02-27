using UnityEngine;

public class DestroyOnFloorHit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DestroyDuck"))
        {
            Destroy(gameObject);
        }
    }
}