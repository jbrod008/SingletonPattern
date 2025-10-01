using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManagerDummy : MonoBehaviour
{
    // Singleton pattern implementation
    public static GameManagerDummy Instance { get; private set; }
    
    [Header("Game Configuration")]
    public int startingLives = 3;
    public int scorePerCoin = 100;
    public float levelTimeLimit = 120f;
    
    [Header("Current Game State")]
    [SerializeField] private GameState currentState = GameState.MainMenu;
    [SerializeField] private int currentLives;
    [SerializeField] private int currentScore;
    [SerializeField] private float remainingTime;
    [SerializeField] private bool isGamePaused = false;
    
    [Header("Game Events")]
    public UnityEvent<int> OnLivesChanged;
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<float> OnTimeChanged;
    public UnityEvent<GameState> OnGameStateChanged;
    public UnityEvent OnGameOver;
    public UnityEvent OnLevelComplete;
    
    [Header("Debug Settings")]
    public bool debugMode = false;
    public KeyCode debugAddScoreKey = KeyCode.F1;
    public KeyCode debugLoseLifeKey = KeyCode.F2;
    
    // Properties for safe external access
    public GameState CurrentState => currentState;
    public int CurrentLives => currentLives;
    public int CurrentScore => currentScore;
    public float RemainingTime => remainingTime;
    public bool IsGamePaused => isGamePaused;
    
    void Awake()
    {
        // Singleton pattern with persistence
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (currentState == GameState.Playing && !isGamePaused)
        {
            UpdateGameTimer();
            HandlePauseInput();
            
            if (debugMode)
            {
                HandleDebugInput();
            }
        }
    }
    
    void InitializeGameManager()
    {
        currentLives = startingLives;
        currentScore = 0;
        remainingTime = levelTimeLimit;
        
        // Initialize UI
        UpdateAllUI();
        
        Debug.Log("GameManager initialized successfully!");
    }
    
    void UpdateGameTimer()
    {
        remainingTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(remainingTime);
        
        if (remainingTime <= 0)
        {
            TimeUp();
        }
    }
    
    void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    void HandleDebugInput()
    {
        if (Input.GetKeyDown(debugAddScoreKey))
        {
            AddScore(1000);
        }
        
        if (Input.GetKeyDown(debugLoseLifeKey))
        {
            LoseLife();
        }
    }
    
    void UpdateAllUI()
    {
        OnLivesChanged?.Invoke(currentLives);
        OnScoreChanged?.Invoke(currentScore);
        OnTimeChanged?.Invoke(remainingTime);
        OnGameStateChanged?.Invoke(currentState);
    }
    
    // Public Methods for Game Control
    public void AddScore(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"Score added: {points}. Total: {currentScore}");
    }
    
    public void LoseLife()
    {
        currentLives--;
        OnLivesChanged?.Invoke(currentLives);
        
        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            Debug.Log($"Life lost! Lives remaining: {currentLives}");
        }
    }
    
    public void StartGame()
    {
        ChangeGameState(GameState.Playing);
        Time.timeScale = 1f;
        remainingTime = levelTimeLimit;
        UpdateAllUI();
        
        Debug.Log("Game started!");
    }
    
    public void PauseGame()
    {
        ChangeGameState(GameState.Paused);
        Time.timeScale = 0f;
        isGamePaused = true;
        
        Debug.Log("Game paused");
    }
    
    public void ResumeGame()
    {
        ChangeGameState(GameState.Playing);
        Time.timeScale = 1f;
        isGamePaused = false;
        
        Debug.Log("Game resumed");
    }
    
    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
        
        Debug.Log("GAME OVER!");
    }
    
    public void CompleteLevel()
    {
        ChangeGameState(GameState.LevelComplete);
        OnLevelComplete?.Invoke();
        
        Debug.Log("Level completed!");
    }
    
    public void RestartGame()
    {
        currentLives = startingLives;
        currentScore = 0;
        remainingTime = levelTimeLimit;
        StartGame();
        
        Debug.Log("Game restarted!");
    }
    
    void TimeUp()
    {
        Debug.Log("Time's up!");
        GameOver();
    }
    
    void ChangeGameState(GameState newState)
    {
        if (currentState == newState) return;
        
        GameState previousState = currentState;
        currentState = newState;
        OnGameStateChanged?.Invoke(currentState);
        
        Debug.Log($"Game state changed: {previousState} → {currentState}");
    }
    
    // Validation method for Inspector
    void OnValidate()
    {
        if (startingLives <= 0)
        {
            Debug.LogWarning("Starting lives should be greater than 0!");
        }
        
        if (levelTimeLimit <= 0)
        {
            Debug.LogWarning("Level time limit should be greater than 0!");
        }
    }
    
    // Called when object is destroyed (cleanup)
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    LevelComplete
}