using UnityEngine;

/// <summary>
/// Handles the movement and shooting of the player ship.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// How quickly the player is able to move forward.
    /// </summary>
    [Tooltip("How quickly the player is able to move forward.")]
    public float thrustSpeed = 1.0f;

    /// <summary>
    /// How quickly the player is able to turn.
    /// </summary>
    [Tooltip("How quickly the player is able to turn.")]
    public float rotationSpeed = 0.1f;

    /// <summary>
    /// The amount of seconds it takes for the player to respawn after dying.
    /// </summary>
    [Tooltip("The amount of seconds it takes for the player to respawn after dying.")]
    public float respawnDelay = 3.0f;

    /// <summary>
    /// The amount of seconds the player has invulnerability after respawning.
    /// This is to prevent the player from instantly dying if spawning into an
    /// asteroid.
    /// </summary>
    [Tooltip("The amount of seconds the player has invulnerability after respawning. This is to prevent the player from instantly dying if spawning into an asteroid.")]
    public float respawnInvulnerability = 3.0f;

    /// <summary>
    /// The object that is cloned when creating a bullet.
    /// </summary>
    [Tooltip("The object that is cloned when creating a bullet.")]
    public Bullet bulletPrefab;

    /// <summary>
    /// The current direction the player is turning. 1=left, -1=right, 0=none
    /// </summary>
    public float turnDirection { get; private set; } = 0.0f;

    /// <summary>
    /// Whether the ship's thrusts are activated causing it to move forward.
    /// </summary>
    public bool thrusting { get; private set; }

    /// <summary>
    /// The rigidbody component attached to the player.
    /// </summary>
    public new Rigidbody2D rigidbody { get; private set; }

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
        // Add force to move the ship forward
        if (this.thrusting) {
            this.rigidbody.AddForce(this.transform.up * this.thrustSpeed);
        }

        // Add torque to rotate the ship
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
