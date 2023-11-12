using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public Bullet bulletPrefab;

    public float thrustSpeed = 1f;
    public bool thrusting { get; private set; }

    public float turnDirection { get; private set; } = 0f;
    public float rotationSpeed = 0.1f;

    public float respawnDelay = 3f;
    public float respawnInvulnerability = 3f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Turn off collisions for a few seconds after spawning to ensure the
        // player has enough time to safely move away from asteroids
        gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
        Invoke(nameof(TurnOnCollisions), respawnInvulnerability);
    }

    private void Update()
    {
        thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            turnDirection = 1f;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            turnDirection = -1f;
        } else {
            turnDirection = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (thrusting) {
            rigidbody.AddForce(transform.up * thrustSpeed);
        }

        if (turnDirection != 0f) {
            rigidbody.AddTorque(rotationSpeed * turnDirection);
        }
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.Project(transform.up);
    }

    private void TurnOnCollisions()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = 0f;
            gameObject.SetActive(false);

            FindObjectOfType<GameManager>().PlayerDeath(this);
        }
    }

}
