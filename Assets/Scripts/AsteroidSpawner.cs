using UnityEngine;

/// <summary>
/// Continuously spawns asteroids and sets their initial trajectory.
/// </summary>
public class AsteroidSpawner : MonoBehaviour
{
    /// <summary>
    /// The object that is cloned when spawning an asteroid.
    /// </summary>
    [Tooltip("The object that is cloned when spawning an asteroid.")]
    public Asteroid asteroidPrefab;

    /// <summary>
    /// The distance the asteroids spawn from the spawner.
    /// </summary>
    [Tooltip("The distance the asteroids spawn from the spawner.")]
    public float spawnDistance = 12.0f;

    /// <summary>
    /// The amount of seconds between spawn cycles.
    /// </summary>
    [Tooltip("The amount of seconds between spawn cycles.")]
    public float spawnRate = 1.0f;

    /// <summary>
    /// The amount of asteroids spawned each cycle.
    /// </summary>
    [Tooltip("The amount of asteroids spawned each cycle.")]
    public int amountPerSpawn = 1;

    /// <summary>
    /// The maximum angle in degrees the asteroid will steer from its initial
    /// trajectory.
    /// </summary>
    [Tooltip("The maximum angle in degrees the asteroid will steer from its initial trajectory.")]
    [Range(0.0f, 45.0f)]
    public float trajectoryVariance = 15.0f;

    private void Start()
    {
        // Start spawning an asteroid at a fixed rate
        InvokeRepeating(nameof(Spawn), this.spawnRate, this.spawnRate);
    }

    public void Spawn()
    {
        for (int i = 0; i < this.amountPerSpawn; i++)
        {
            // Choose a random direction from the center of the spawner and
            // spawn the asteroid a distance away
            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPoint = spawnDirection * this.spawnDistance;

            // Offset the spawn point by the position of the spawner so its
            // relative to the spawner location
            spawnPoint += this.transform.position;

            // Calculate a random variance in the asteroid's rotation which will
            // cause its trajectory to change
            float variance = Random.Range(-this.trajectoryVariance, this.trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            // Create the new asteroid by cloning the prefab and set a random
            // size within the range
            Asteroid asteroid = Instantiate(this.asteroidPrefab, spawnPoint, rotation);
            asteroid.size = Random.Range(asteroid.minSize, asteroid.maxSize);

            // The asteroid will float towards the spawner location
            Vector2 trajectory = rotation * -spawnDirection;
            asteroid.SetTrajectory(trajectory);
        }
    }

}
