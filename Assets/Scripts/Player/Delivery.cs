using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] ParticleSystem PackageParticles;

    bool hasPackage = false;

    //Events
    public delegate void PickupDelegate(string pickupType);
    public static event PickupDelegate OnPickup;

    private void Awake()
    {
        PackageParticles.Stop();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasPackage && collision.CompareTag("Pickup"))
        {
            Debug.Log("Grabbed Package");
            hasPackage = true;
            PackageParticles.Play();
            Destroy(collision.gameObject);
            OnPickup?.Invoke("Package");
        }
        if (hasPackage && collision.CompareTag("Customer"))
        {
            Debug.Log("Touched Customer");
            PackageParticles.Stop();
            hasPackage = false;
            Destroy(collision.gameObject);
            OnPickup?.Invoke("Customer");
        }
        if (collision.CompareTag("Boost"))
        {
            OnPickup?.Invoke("Boost");
            Destroy(collision.gameObject);
        }
    }
}
