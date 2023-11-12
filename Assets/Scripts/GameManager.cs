using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player player;
    public ParticleSystem explosionEffect;
    public GameObject gameOverUI;

    public int score { get; private set; }
    public Text scoreText;

    public int lives { get; private set; }
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
