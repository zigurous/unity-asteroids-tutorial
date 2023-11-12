using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public Bullet bulletPrefab;

    public float thrustSpeed = 1.0f;
    public bool thrusting { get; private set; }

    public float turnDirection { get; private set; } = 0.0f;
    public float rotationSpeed = 0.1f;

    public float respawnDelay = 3.0f;
    public float respawnInvulnerability = 3.0f;

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Turn off collisions for a few seconds after spawning to ensure the
        // player has enough time to safely move away from asteroids
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");
        Invoke(nameof(TurnOnCollisions), this.respawnInvulnerability);
    }

    private void Update()
    {
        this.thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            this.turnDirection = 1.0f;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            this.turnDirection = -1.0f;
        } else {
            this.turnDirection = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        if (this.thrusting) {
            this.rigidbody.AddForce(this.transform.up * this.thrustSpeed);
        }

        if (this.turnDirection != 0.0f) {
            this.rigidbody.AddTorque(this.rotationSpeed * this.turnDirection);
        }
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);
        bullet.Project(this.transform.up);
    }

    private void TurnOnCollisions()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Asteroid")
        {
            this.rigidbody.velocity = Vector3.zero;
            this.rigidbody.angularVelocity = 0.0f;
            this.gameObject.SetActive(false);

            FindObjectOfType<GameManager>().PlayerDeath(this);
        }
    }

}
