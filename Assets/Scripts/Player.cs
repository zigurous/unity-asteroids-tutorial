using UnityEngine;

/// <summary>
/// Handles the movement and shooting of the
/// player ship.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
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
    /// The object that is cloned when creating a bullet.
    /// </summary>
    [Tooltip("The object that is cloned when creating a bullet.")]
    public Bullet bulletPrefab;

    /// <summary>
    /// The current direction the player is turning.
    /// 1=left, -1=right, 0=none
    /// </summary>
    private float _turnDirection = 0.0f;

    /// <summary>
    /// Whether the ship's thrusts are activated causing
    /// it to move forward.
    /// </summary>
    private bool _thrusting = false;

    /// <summary>
    /// The rigidbody component attached to the player.
    /// </summary>
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        // Store references to the player's components
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Turn off collisions for a few seconds after
        // spawning to ensure the player has enough
        // time to safely move away from asteroids
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Collisions");

        Invoke(nameof(TurnOnCollisions), 3.0f);
    }

    private void Update()
    {
        // Activate thrust when pressing the 'w' key or 'up arrow' key
        _thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        // Set the turn direction of the ship based on
        // which input key is being held
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            _turnDirection = 1.0f;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            _turnDirection = -1.0f;
        } else {
            _turnDirection = 0.0f;
        }

        // Shoot a bullet each time the 'space' key is pressed
        // or when the mouse left button is clicked
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        // Add force to move the ship forward
        if (_thrusting) {
            _rigidbody.AddForce(this.transform.up * this.thrustSpeed);
        }

        // Add torque to rotate the ship
        if (_turnDirection != 0.0f) {
            _rigidbody.AddTorque(this.rotationSpeed * _turnDirection);
        }
    }

    private void Shoot()
    {
        // Spawn a bullet and project it the direction the player is aiming
        Bullet bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);
        bullet.Project(this.transform.up);
    }

    private void TurnOnCollisions()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player crashed into an asteroid
        if (collision.gameObject.tag == "Asteroid")
        {
            // Stop all movement of the ship
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = 0.0f;

            // Stop all player controls and rendering of the ship
            this.gameObject.SetActive(false);

            // Inform the game manager the player has died
            // so the lives can be updated along with any
            // other state changes
            FindObjectOfType<GameManager>().PlayerDeath(this);
        }
    }

}
