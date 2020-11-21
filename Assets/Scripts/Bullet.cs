using UnityEngine;

/// <summary>
/// Handles the physics/movement of a bullet
/// projectile.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// How fast the bullet travels.
    /// </summary>
    [Tooltip("How fast the bullet travels.")]
    public float speed = 100.0f;

    /// <summary>
    /// The maximum amount of time the bullet will
    /// stay alive after being projected.
    /// </summary>
    [Tooltip("The maximum amount of time the bullet will stay alive after being projected.")]
    public float maxLifetime = 10.0f;

    /// <summary>
    /// The rigidbody component attached to the bullet.
    /// </summary>
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        // Store references to the bullet's components
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction)
    {
        // Move the bullet in the desired direction
        // while factoring in the speed of the bullet
        _rigidbody.AddForce(direction * this.speed);

        // Destroy the bullet after it reaches its max lifetime
        Destroy(this.gameObject, this.maxLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet if it collides with anything
        Destroy(this.gameObject);
    }

}
