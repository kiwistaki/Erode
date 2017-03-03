using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour {

    public List<GameObject> Highscores;

    private static List<Highscore> _highscores = new List<Highscore>();
    private Highscore _currentHighscore;
    private GameObject _currentUIHighscore;
    private Text _blinkingLetter;
    private float _blinktimer = 0;

	// Use this for initialization
	private void Start ()
    {
        if (_highscores.Count == 0)
        {
            foreach (GameObject h in Highscores)
            {
                Text[] highscoreElems = h.GetComponentsInChildren<Text>();
                _highscores.Add(new Highscore(Convert.ToInt32(highscoreElems[1].text), 0, '-'));
            }
        }
	}

    private void Update()
    {
        _blinktimer += Time.unscaledDeltaTime;
        if (_blinkingLetter != null)
        {
            float ms = _blinktimer - (float)Math.Floor(_blinktimer);
            if (ms > 0.5)
                _blinkingLetter.enabled = false;
            else
                _blinkingLetter.enabled = true;
        }
        UpdateHighScores();
    }

    public bool IsHighScore(int score) 
    {
        return score > Convert.ToInt32(Highscores[Highscores.Count - 1].GetComponentsInChildren<Text>()[1]); // get the last highscore
    }

    public void AddHighscore(int score, float timer)
    {
        bool isSmaller = true;
        int i = 0;
        for(; i < _highscores.Count && isSmaller; i++)
        {
            if (_highscores[i]._score < score)
                isSmaller = false;
        }
        _currentHighscore = new Highscore(score, timer, 'A');
        _currentUIHighscore = Highscores[i - 1];
        _blinkingLetter = _currentUIHighscore.GetComponentsInChildren<Text>()[2];
        _highscores.Insert(i - 1, _currentHighscore);
        _highscores.RemoveAt(_highscores.Count - 1);
    }

    public void ChangeCurrentHighscoreLetter(int letterIndex, bool moveUp)
    {
        switch(letterIndex)
        {
            case 1:
                _currentHighscore._letter1 = GetNextLetter(_currentHighscore._letter1, moveUp);
                break;
            case 2:
                _currentHighscore._letter2 = GetNextLetter(_currentHighscore._letter2, moveUp);
                break;
            case 3:
                _currentHighscore._letter3 = GetNextLetter(_currentHighscore._letter3, moveUp);
                break;
            default:
                Debug.Log("WRONG LETTER INDEX");
                break;
        }
    }

    public void ChangeCurrentHighscoreBlinkingLetter(int letterIndex)
    {
        if (letterIndex > 0 && letterIndex < 4)
        {
            _blinkingLetter.enabled = true;
            _blinkingLetter = _currentUIHighscore.GetComponentsInChildren<Text>()[letterIndex + 1];
        }
    }

    public void StopBlinkingLetter()
    {
        _blinkingLetter.enabled = true;
        _blinkingLetter = null;
    }

    private char GetNextLetter(char c, bool moveUp)
    {
        char temp = c;
        if (temp == 'A' && !moveUp)
            return 'Z';
        if (temp == 'Z' && moveUp)
            return 'A';
        return moveUp ? ++temp : --temp;
    }

    private void UpdateHighScores()
    {
        for(int i = 0; i < Highscores.Count; i++)
        {
            Text[] highscoreElems = Highscores[i].GetComponentsInChildren<Text>();
            Highscore currentHighscore = _highscores[i];
            highscoreElems[1].text = currentHighscore._score.ToString();
            highscoreElems[2].text = currentHighscore._letter1.ToString();
            highscoreElems[3].text = currentHighscore._letter2.ToString();
            highscoreElems[4].text = currentHighscore._letter3.ToString();
            highscoreElems[5].text = currentHighscore.GetTimerFormat();
        }
    }
}
