using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {


    private  int _timeScore;
    private int _destroyScore;
    public  int scoreMultiplier = 1;
    public  int difficultyMultiplier = 1;

    private int _totalScore = 0;
    private int _levelScore = 0;
    private string _levelName = "";

    public GameObject PointPopup;

    #region pointValues
    public const int scorePerSecond = 50;

    public const int asteroidHitScore = 50;
    public const int hunterKillScore = 200;
    public const int shooterKillScore = 300;
    public const int chargerKillScore = 400;

    public const int empDestroyScore = 300;
    
    #endregion

    public enum ScoreType 
    {
        Undefined = -1, 
        Hunter,
        Shooter,
        Charger,
        Asteroid,
        EMP
    }

    void Awake() 
    {
    }

    // Use this for initialization
	void Start () {

        _timeScore = 0;
        _destroyScore = 0;
        _totalScore = 0;
        _levelScore = 0;
        _levelName = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public  void initializePointsForNewGame()
    {
        _timeScore = 0;
        _destroyScore = 0;
        scoreMultiplier = 1;
    }

    public void ChangeLevel(string levelName)
    {
        _levelName = levelName;
        _levelScore = 0;
    }

    public void UpdateTimeScore(float deltaTime)
    {
        _timeScore += (int)(scorePerSecond * deltaTime * scoreMultiplier);
        _levelScore += (int)(scorePerSecond * deltaTime * scoreMultiplier);
    }

    public  void UpdateTotalScore() 
    {
        _totalScore = (_timeScore + _destroyScore) * difficultyMultiplier;
    }

    public  void showScoreOnDestroy(ScoreType type, Vector3 dedPos) 
    {
        GameObject points;
        switch (type) 
        {
            case ScoreManager.ScoreType.Asteroid:
                points = Instantiate(this.PointPopup, dedPos, Quaternion.identity);
                points.GetComponent<TextMesh>().text = asteroidHitScore.ToString();
                
                break;
            case ScoreManager.ScoreType.Hunter:
                points = Instantiate(this.PointPopup, dedPos, Quaternion.identity);
                points.GetComponent<TextMesh>().text = hunterKillScore.ToString();
               
                break;
            case ScoreManager.ScoreType.Shooter:
                points = Instantiate(this.PointPopup, dedPos, Quaternion.identity);
                points.GetComponent<TextMesh>().text = shooterKillScore.ToString();
               
                break;
            case ScoreManager.ScoreType.Charger:
                points = Instantiate(this.PointPopup, dedPos, Quaternion.identity);
                points.GetComponent<TextMesh>().text = chargerKillScore.ToString();
              
                break;
            case ScoreManager.ScoreType.EMP:
                points = Instantiate(this.PointPopup, dedPos, Quaternion.identity);
                points.GetComponent<TextMesh>().text = empDestroyScore.ToString();
              
                break;
        }

        
    }

    public  int getScore() 
    {
        return _totalScore;
    }

    public int getLevelScore()
    {
        return _levelScore;
    }

    public void IncrementDestroyScore(ScoreType type, string levelName) 
    {
        int score = 0;
        switch (type) 
        {
            case ScoreManager.ScoreType.Asteroid:
                score += asteroidHitScore * difficultyMultiplier;
                break;
            case ScoreManager.ScoreType.Hunter:
                score += hunterKillScore * difficultyMultiplier;
                break;
            case ScoreManager.ScoreType.Shooter:
                score += shooterKillScore * difficultyMultiplier;
                break;
            case ScoreManager.ScoreType.Charger:
                score += chargerKillScore * difficultyMultiplier;
                break;
            case ScoreManager.ScoreType.EMP:
                score += empDestroyScore * difficultyMultiplier;
                break;
        }
        _destroyScore += score;
        if (_levelName == levelName)
            _levelScore += score;
    }
}
