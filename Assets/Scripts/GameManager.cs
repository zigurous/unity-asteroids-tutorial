using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the state of the game such as scoring, dying, and game over.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The player component.
    /// </summary>
    [Tooltip("The player component.")]
    public Player player;

    /// <summary>
    /// The particle effect that is played when an asteroid is destroyed and
    /// when the player dies.
    /// </summary>
    [Tooltip("The particle effect that is played when an asteroid is destroyed and when the player dies.")]
    public ParticleSystem explosionEffect;

    /// <summary>
    /// The UI displayed during the game over state.
    /// </summary>
    [Tooltip("The UI displayed during the game over state.")]
    public GameObject gameOverUI;

    /// <summary>
    /// The current score of the player.
    /// </summary>
    public int score { get; private set; }

    /// <summary>
    /// The UI text that displays the player's score.
    /// </summary>
    [Tooltip("The UI text that displays the player's score.")]
    public Text scoreText;

    /// <summary>
    /// The current amount of lives of the player.
    /// </summary>
    public int lives { get; private set; }

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
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    public void NewGame()
    {
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();

        // Clear the scene of asteroids so we start fresh
        for (int i = 0; i < asteroids.Length; i++) {
            Destroy(asteroids[i].gameObject);
        }

        this.gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        Respawn();
    }

    public void Respawn()
    {
        this.player.transform.position = Vector3.zero;
        this.player.gameObject.SetActive(true);
    }

    public void AsteroidDestroyed(Asteroid asteroid)
    {
        this.explosionEffect.transform.position = asteroid.transform.position;
        this.explosionEffect.Play();

        // Score is increased based on the size of the asteroid
        if (asteroid.size < 0.7f) {
            SetScore(this.score + 100); // small asteroid
        } else if (asteroid.size < 1.4f) {
            SetScore(this.score + 50); // medium asteroid
        } else {
            SetScore(this.score + 25); // large asteroid
        }
    }

    public void PlayerDeath(Player player)
    {
        this.explosionEffect.transform.position = player.transform.position;
        this.explosionEffect.Play();

        SetLives(this.lives - 1);

        if (this.lives <= 0) {
            GameOver();
        } else {
            Invoke(nameof(Respawn), player.respawnDelay);
        }
    }

    public void GameOver()
    {
        this.gameOverUI.SetActive(true);
    }

    private void SetScore(int score)
    {
        this.score = score;
        this.scoreText.text = score.ToString();
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        this.livesText.text = lives.ToString();
    }

}
