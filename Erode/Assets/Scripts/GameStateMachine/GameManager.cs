using Assets.Scripts.Control;
using Assets.Scripts.HexGridGenerator;
using Assets.Scripts.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public PlayerController Player;
    public Grid Grid;
    public BoxCollider EndGameBoxCollider;
    public Text TimerValue;
    public Text ScoreValue;
    public Text FinalScore;
    

    public static int timeScore;
    public static int destroyScore;
    public static int scoreMultiplier = 1;
    public static int difficultyMultiplier = 1;

    public static int totalScore = (timeScore + destroyScore) * difficultyMultiplier;
    

    private float _gameTimer;
    private float _lastTimeScale;
    private Canvas _mainMenuCanvas;
    private Canvas _HUDCanvas;
    private Canvas _pauseCanvas;
    private Canvas _endGameCanvas;
    private LeaderboardManager _leaderboardManager;

    private Context _context;

    private Vector3 _cameraInitialPosition;
    private Quaternion _cameraInitialRotation;
    private Vector3 _cameraInitialLocalScale;

    // Use this for initialization
    void Start () {
        _mainMenuCanvas = GameObject.Find("MainMenuCanvas").GetComponent<Canvas>();
        _HUDCanvas = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
        _pauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        _endGameCanvas = GameObject.Find("EndGameCanvas").GetComponent<Canvas>();
        _leaderboardManager = GameObject.Find("MainCamera").GetComponent<LeaderboardManager>();

        _mainMenuCanvas.enabled = true;
        _HUDCanvas.enabled = false;
        _pauseCanvas.enabled = false;
        _endGameCanvas.enabled = false;

        

        Transform _cameraTransform = GameObject.Find("MainCamera").GetComponent<Transform>();
        _cameraInitialPosition = _cameraTransform.localPosition;
        _cameraInitialRotation = _cameraTransform.localRotation;
        _cameraInitialLocalScale = _cameraTransform.localScale;

        _context = new Context(new NewGameState(this));

        timeScore = 0;
        destroyScore = 0;
        totalScore = 0;

        //Il faut augmenter les points liés au temps de cette facon, afin que le multiplicateur de score fonctionne
        InvokeRepeating("UpdateScore", 0f, 1.0f);
        Time.timeScale = 0f;
        Initialize();
    }

    void Initialize()
    {
        Player.ResetPlayer();
        Input.ResetInputAxes();
        Transform _cameraTransform = GameObject.Find("MainCamera").GetComponent<Transform>();
        _cameraTransform.localPosition = _cameraInitialPosition;
        _cameraTransform.localRotation = _cameraInitialRotation;
        _cameraTransform.localScale = _cameraInitialLocalScale;

        _gameTimer = 0;
        _lastTimeScale = Time.timeScale;

        Grid.GenerateGrid();
    }
	
	// Update is called once per frame
	void Update () {
        _context.Request(this, _leaderboardManager);
	}

    public void GameOverRequest()
    {
        _context = new Context(new GameOverState(this));
    }

    void UpdateGameTimer()
    {
        float timeScale = Time.timeScale;
        _gameTimer += Utils.getRealDeltaTime();
        int min = (int)_gameTimer / 60; // calculate the minutes
        int sec = (int) _gameTimer % 60; // calculate the seconds

        TimerValue.text = min < 10 ? "0" + min : min.ToString();
        TimerValue.text += ":";
        TimerValue.text += sec < 10 ? "0" + sec : sec.ToString();
                
        totalScore = (timeScore + destroyScore) * difficultyMultiplier;

        ScoreValue.text = totalScore.ToString();

    }

    //Il faut augmenter les points liés au temps de cette facon, afin que le multiplicateur de score fonctionne
    void UpdateScore() 
    {
        timeScore += 1*scoreMultiplier;
    }

    void WaitStartNewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Function to call to disable the mainmenu and start the game
    void StartNewGame()
    {
        Initialize();

        // Manage canvas
        _mainMenuCanvas.enabled = false;
        _pauseCanvas.enabled = false;
        _HUDCanvas.enabled = true;
        _endGameCanvas.enabled = false;

        // Manage in-game property
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 1.0f;
        EndGameBoxCollider.enabled = true;

        timeScore = 0;
        destroyScore = 0;
        scoreMultiplier = 1;
    }

    void PauseGame()
    {
        // Manage canvas
        _mainMenuCanvas.enabled = false;
        _pauseCanvas.enabled = true;
        _HUDCanvas.enabled = false;
        _endGameCanvas.enabled = false;

        // Manage in-game property
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    void UnpauseGame()
    {
        // Manage canvas
        _mainMenuCanvas.enabled = false;
        _pauseCanvas.enabled = false;
        _HUDCanvas.enabled = true;
        _endGameCanvas.enabled = false;

        // Manage in-game property
        Time.timeScale = _lastTimeScale;
    }

    bool EndGame()
    {
        // Manage canvas
        _mainMenuCanvas.enabled = false;
        _HUDCanvas.enabled = false;
        _pauseCanvas.enabled = false;
        _endGameCanvas.enabled = true;

        // Manage in-game property
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        EndGameBoxCollider.enabled = false;

        // Manage private property
        FinalScore.text = ScoreValue.text;

        if(_leaderboardManager.IsHighScore(Convert.ToInt32(ScoreValue.text)))
        {
            GameObject.Find("NewHighscoreText").GetComponent<Text>().enabled = true;
            _leaderboardManager.AddHighscore(Convert.ToInt32(ScoreValue.text), _gameTimer);
            return true;
        }

        return false;
    }

    #region States

    /// <summary>
    /// The 'State' abstract class
    /// </summary>
    abstract class State
    {
        public abstract void Handle(Context context, GameManager g_manager, LeaderboardManager l_manager);
    }

    class NewGameState : State
    {
        public NewGameState(GameManager manager) { }
        public override void Handle(Context context, GameManager g_manager, LeaderboardManager l_manager)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                g_manager.StartNewGame();
                context.State = new InGameState(g_manager);
            }
        }
    }

    class InGameState : State
    {
        public InGameState(GameManager manager) {  }
        public override void Handle(Context context, GameManager g_manager, LeaderboardManager l_manager)
        {
            g_manager.UpdateGameTimer();
            if (Input.GetButtonDown("Pause"))
            {
                g_manager.PauseGame();
                context.State = new PauseGameState(g_manager);
            }
        }
    }

    class PauseGameState : State
    {
        public PauseGameState(GameManager manager) { }
        public override void Handle(Context context, GameManager g_manager, LeaderboardManager l_manager)
        {
            if (Input.GetButtonDown("Pause"))
            {
                g_manager.UnpauseGame();
                context.State = new InGameState(g_manager);
            }
        }
    }

    class GameOverState : State
    {
        private int _highscoreLetter = 1;
        private bool _isHighscore = false;
        private bool _axisInUse = false;
        private GameObject restartButton = GameObject.Find("RestartButton");

        public GameOverState(GameManager manager)
        {
           _isHighscore = manager.EndGame();
            if (_isHighscore)
                restartButton.SetActive(false);
        }
        public override void Handle(Context context, GameManager g_manager, LeaderboardManager l_manager)
        {
            if(_isHighscore)
            {
                if (!_axisInUse)
                {
                    if (Input.GetAxisRaw("PadVertical") > 0) // Up
                    {
                        _axisInUse = true;
                        l_manager.ChangeCurrentHighscoreLetter(_highscoreLetter, true);
                    }
                    if (Input.GetAxisRaw("PadVertical") < 0) // Down
                    {
                        _axisInUse = true;
                        l_manager.ChangeCurrentHighscoreLetter(_highscoreLetter, false);
                    }
                    if (Input.GetAxisRaw("PadHorizontal") < 0) // Left
                    {
                        _axisInUse = true;
                        if (_highscoreLetter > 1)
                        {
                            l_manager.ChangeCurrentHighscoreBlinkingLetter(--_highscoreLetter);
                        }
                    }
                    if (Input.GetAxisRaw("PadHorizontal") > 0) // Right
                    {
                        _axisInUse = true;
                        if (_highscoreLetter >= 3)
                        {
                            // STOP BLINKING AND ACTIVATE BUTTON
                            l_manager.StopBlinkingLetter();
                           restartButton.SetActive(true);
                            _isHighscore = false;
                        }
                        l_manager.ChangeCurrentHighscoreBlinkingLetter(++_highscoreLetter);
                    }
                }
                else
                {
                    if (Input.GetAxisRaw("PadHorizontal") == 0 && Input.GetAxisRaw("PadVertical") == 0)
                        _axisInUse = false;
                }
            }
            else
            {
                if (Input.GetButtonUp("Fire1"))
                {
                    g_manager.WaitStartNewGame();
                    context.State = new NewGameState(g_manager);
                }
            }
        }
    }

    class Context
    {
        private State _state;
        // Constructor
        public Context(State state)
        {
            this.State = state;
        }

        // Gets or sets the state
        public State State
        {
            get { return _state; }
            set
            {
                _state = value;
            }
        }

        public void Request(GameManager g_manager, LeaderboardManager l_manager)
        {
            _state.Handle(this, g_manager, l_manager);
        }
    }

    #endregion
}
