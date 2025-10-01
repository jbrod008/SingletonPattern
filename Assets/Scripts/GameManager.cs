
using UnityEngine;
using UnityEngine.UI;
using TMPro; //Namesapce for textmeshpro
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Game Stats")]
    public int score = 0;//score is calculated
    public int lives = 3;
    public int enemiesKilled = 0;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text enemiesKilledText;
    public GameObject gameOverPanel;
    //public TMP_Text scoreText;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManagers
        }
    }

    private void Start()
    {
        RefreshUIReferences();
        UpdateUI();

    }

    private void RefreshUIReferences()
    {
        if (scoreText == null)
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        if (livesText == null)
            livesText = GameObject.Find("LivesText")?.GetComponent<TMP_Text>();
        if (enemiesKilledText == null)
            enemiesKilledText = GameObject.Find("EnemiesText")?.GetComponent<TMP_Text>();
        if (gameOverPanel == null)
            gameOverPanel = GameObject.Find("GameOverPanel");
    }
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Score increased by {points}. Total: {score}");
        UpdateUI();
     }

    
    public void LoseLife()
    {
        lives--;
        Debug.Log($"Life lost! Lives remaining: {lives}");
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        AddScore(100); // 100 points per enemy
        Debug.Log($"Enemy killed! Total enemies defeated: {enemiesKilled}");
    }


    public void CollectiblePickedUp(int value)
    {
        AddScore(value);
        Debug.Log($"Collectible picked up worth {value} points!");
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (livesText) livesText.text = "Lives: " + lives;
        if (enemiesKilledText) enemiesKilledText.text = "Enemies: " + enemiesKilled;
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void reloadGame()
    {
        //SceneManager.LoadScene("Delete");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quitGame()
    {
        Application.Quit();
    }


    public void RestartGame()
    {
        /* score = 0;
         lives = 3;
         enemiesKilled = 0;
         Time.timeScale = 1f;
         if (gameOverPanel) gameOverPanel.SetActive(false);
         UpdateUI();*/


        // CRITICAL: Unpause the game first!
        Time.timeScale = 1f;

        // Reset all game state
        score = 0;
        lives = 3;
        enemiesKilled = 0;

        // Hide game over panel
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Destroy all enemies, bullets, and collectibles before reloading
        DestroyAllGameObjects();

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void DestroyAllGameObjects()
    {
        // Destroy all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        // Destroy all bullets
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        // Destroy all collectibles
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject collectible in collectibles)
        {
            Destroy(collectible);
        }
    }
}
