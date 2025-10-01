using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public int value = 50;
    public float rotationSpeed = 90f;

    private void Update()
    {
        // Rotate for visual effect
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Easy access to GameManager through Singleton!
            GameManager.Instance.CollectiblePickedUp(value);
            Destroy(gameObject);
        }
    }
}