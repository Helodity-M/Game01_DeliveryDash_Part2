using UnityEngine;

public class Delivery : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            Debug.Log("Grabbed Package");
        }
        if (collision.CompareTag("Customer"))
        {
            Debug.Log("Touched Customer");
        }
    }
}
