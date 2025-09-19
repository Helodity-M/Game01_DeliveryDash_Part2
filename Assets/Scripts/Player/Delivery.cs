using UnityEngine;

public class Delivery : MonoBehaviour
{
    bool hasPackage = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasPackage && collision.CompareTag("Pickup"))
        {
            Debug.Log("Grabbed Package");
            hasPackage = true;
            Destroy(collision.gameObject, 0.2f);
        }
        if (hasPackage && collision.CompareTag("Customer"))
        {
            Debug.Log("Touched Customer");
            hasPackage = false;
        }
    }
}
