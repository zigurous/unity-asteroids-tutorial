using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public float thrustSpeed = 1.0f;
    public float rotationSpeed = 0.1f;
    public float shootCooldown = 0.15f;
    public Bullet bulletPrefab;

    private float _turnDirection = 0.0f;
    private bool _thrusting = false;
    private bool _shootingCooldown = false;

    /// <summary>
    /// The rigidbody component attached to the player.
    /// </summary>
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _thrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            _turnDirection = 1.0f;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            _turnDirection = -1.0f;
        } else {
            _turnDirection = 0.0f;
        }

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
        // Prevent shooting if on cooldown
        if (_shootingCooldown) {
            return;
        }

        // Spawn a bullet and project it the
        // direction the player is aiming
        Bullet bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);
        bullet.Project(this.transform.up);

        // Start a cooldown, if set, to prevent the
        // player from shooting too rapidly
        if (this.shootCooldown > 0.0f)
        {
            _shootingCooldown = true;
            Invoke(nameof(EndShootingCooldown), this.shootCooldown);
        }
    }

    private void EndShootingCooldown()
    {
        _shootingCooldown = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player crashed into an asteroid
        if (collision.gameObject.tag == "Asteroid")
        {
            this.gameObject.SetActive(false);

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = 0.0f;

            // Inform the game manager the player has died
            // so the lives can be updated along with any
            // other state changes
            FindObjectOfType<GameManager>().PlayerDeath(this);
        }
    }

}
