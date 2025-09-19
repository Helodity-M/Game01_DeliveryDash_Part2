using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] ParticleSystem PackageParticles;

    bool hasPackage = false;

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
            Destroy(collision.gameObject, 0.2f);
        }
        if (hasPackage && collision.CompareTag("Customer"))
        {
            Debug.Log("Touched Customer");
            PackageParticles.Stop();
            hasPackage = false;
        }
    }
}
