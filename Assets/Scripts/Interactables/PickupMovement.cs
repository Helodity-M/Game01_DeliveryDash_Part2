using UnityEngine;

public class PickupMovement : MonoBehaviour
{
    [SerializeField] float BobAmount;
    [SerializeField] float BobSpeed;
    Vector3 initialPosition;
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = initialPosition + Vector3.up * Mathf.Sin(Time.time * BobSpeed) * BobAmount;
    }
}
