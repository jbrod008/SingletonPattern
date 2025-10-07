using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip CoinSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource for sound effects
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f; // Adjust volume as needed

    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        rb.linearVelocity = movement * moveSpeed;
    }

    private void HandleShooting()
    {

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;
        }

    }

    private void FireBullet()
    {
        if (GameManager.Instance.score > 400 && GameManager.Instance.score < 1000)
            fireRate = 0.3f;
        if( GameManager.Instance.score > 900)
            fireRate = 0.1f;
        if (bulletPrefab && firePoint)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        if (GameManager.Instance.score > 2000)
            GameManager.Instance.GameWon();

        // Play shoot sound effect
        audioSource.PlayOneShot(shootSound);  
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Player hit by enemy - lose a life
            GameManager.Instance.LoseLife();
     
        }

        if (other.CompareTag("Collectible"))
        {
            // Player collected an item
            Collectible collectible = other.GetComponent<Collectible>();
            if (collectible)
            {
               
                Destroy(other.gameObject);
            }
        }
    }
}
