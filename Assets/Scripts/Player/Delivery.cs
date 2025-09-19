using UnityEngine;

public class Delivery : MonoBehaviour
{
    bool hasPackage = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            Debug.Log("Grabbed Package");
            hasPackage = true;
        }
        if (hasPackage && collision.CompareTag("Customer"))
        {
            Debug.Log("Touched Customer");
            hasPackage = false;
        }
    }
}
