using UnityEngine;

public class Collision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision!");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger!!!");
    }
}
