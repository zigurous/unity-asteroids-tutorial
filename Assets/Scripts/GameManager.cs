using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the state of the game, such as
/// scoring, dying, and starting a new game.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The player component.
    /// </summary>
    [Tooltip("The player component.")]
    public Player player;

    /// <summary>
    /// The particle effect that is played when an
    /// asteroid is destroyed and when the player dies.
    /// </summary>
    [Tooltip("The particle effect that is played when an asteroid is destroyed and when the player dies.")]
    public ParticleSystem explosionEffect;

    /// <summary>
    /// The UI displayed during the game over state.
    /// </summary>
    [Tooltip("The UI displayed during the game over state.")]
    public GameObject gameOverUI;

    private int _score = 0;

    /// <summary>
    /// The current score of the player.
    /// </summary>
    public int score
    {
        get => _score;
        private set
        {
            _score = value;
            this.scoreText.text = _score.ToString();
        }
    }

    /// <summary>
    /// The UI text that displays the player's score.
    /// </summary>
    [Tooltip("The UI text that displays the player's score.")]
    public Text scoreText;

    private int _lives = 3;

    /// <summary>
    /// The current amount of lives of the player.
    /// </summary>
    public int lives
    {
        get => _lives;
        private set
        {
            _lives = value;
            this.livesText.text = _lives.ToString();
        }
    }

    /// <summary>
    /// The UI text that displays the player's lives.
    /// </summary>
    [Tooltip("The UI text that displays the player's lives.")]
    public Text livesText;

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        // Start a new game once the player presses 'Return'
        if (_lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    public void NewGame()
    {
        // Clear the scene of asteroids so we can start fresh
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();
        for (int i = 0 ; i < asteroids.Length; i++) {
            Destroy(asteroids[i].gameObject);
        }

        // Hide the game over UI
        this.gameOverUI.SetActive(false);

        // Reset score and lives
        this.score = 0;
        this.lives = 3;

        // Spawn the player
        Respawn();
    }

    public void Respawn()
    {
        // Move the player back to the center
        // and reactivate their controls
        this.player.transform.position = Vector3.zero;
        this.player.gameObject.SetActive(true);
    }

    public void AsteroidDestroyed(Asteroid asteroid)
    {
        // Play the explosion effect at the location of the asteroid
        this.explosionEffect.transform.position = asteroid.transform.position;
        this.explosionEffect.Play();

        // Increase the score based on the size of the asteroid
        if (asteroid.size < 0.7f) {
            this.score += 100; // small asteroid
        } else if (asteroid.size < 1.4f) {
            this.score += 50; // medium asteroid
        } else {
            this.score += 20; // large asteroid
        }
    }

    public void PlayerDeath(Player player)
    {
        // Play the explosion effect at the
        // location of the player
        this.explosionEffect.transform.position = player.transform.position;
        this.explosionEffect.Play();

        // Decrement lives
        this.lives--;

        // Check if a game over state has been reached (no lives)
        if (this.lives <= 0) {
            GameOver();
        } else {
            // Respawn the player if they have more lives
            Invoke(nameof(Respawn), 3.0f);
        }
    }

    public void GameOver()
    {
        // Show the game over UI
        this.gameOverUI.SetActive(true);
    }

}
