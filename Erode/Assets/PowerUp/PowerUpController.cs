using Assets.Scripts.Control;
using Assets.Scripts.iTween;
using Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PowerUp
{
    class PowerUpController : MonoBehaviour
    {
        public PlayerController PlayerController;

        private int _superSpeedCount = 0, _slowMotionCount = 0, _scoreMultCount = 0;
        private List<float> _superSpeedTimers = new List<float>(), _scoreMultTimers = new List<float>();
        private float _slowMotionTimer = 0f;
        private Image _superSpeedIcon, _slowMotionIcon, _slowMotionBorder, _scoreMultIcon;
        private Text _superSpeedTimerText, _slowMotionTimerText, _scoreMultTimerText,
            _superSpeedCountText, _scoreMultCountText;
        private Canvas _HUDCanvas;
        private GameObject _bonusesPanel;
        private RectTransform _bonusesPanelRT;

        private ScoreManager _scoreManager;

        public void ActivatePowerUpEffect(AbstractPowerUp.PowerUpType type, float duration)
        {
            switch(type)
            {
                case AbstractPowerUp.PowerUpType.SuperSpeed:
                    _superSpeedCount += 1;
                    PlayerController.SpeedModifier += 0.5f;
                    _superSpeedTimers.Insert(_superSpeedTimers.Count, duration);
                    goto default;
                case AbstractPowerUp.PowerUpType.SlowMotion:
                    _slowMotionCount += 1;
                    _slowMotionTimer = duration;
                    Time.timeScale = .3f;
                    PlayerController.PlayerAnimator.speed = 1 / Time.timeScale;
                    goto default;
                case AbstractPowerUp.PowerUpType.ScoreMultiplier:
                    _scoreMultCount += 1;
                    _scoreManager.scoreMultiplier *= 2;
                    _scoreMultTimers.Insert(_scoreMultTimers.Count, duration);
                    goto default;
                default:
                    ShowBonusInUI(type);
                    break;
            }
        }

        public void DeactivatePowerUpEffect(AbstractPowerUp.PowerUpType type)
        {
            switch (type)
            {
                case AbstractPowerUp.PowerUpType.SuperSpeed:
                    _superSpeedCount -= 1;
                    PlayerController.SpeedModifier -= 0.5f;
                    _superSpeedTimers.RemoveAt(0);
                    goto default;
                case AbstractPowerUp.PowerUpType.SlowMotion:
                    _slowMotionCount -= 1;
                    if(_slowMotionCount == 0)
                    {
                        Time.timeScale = 1f;
                        PlayerController.PlayerAnimator.speed = Time.timeScale;
                    }
                    goto default;
                case AbstractPowerUp.PowerUpType.ScoreMultiplier:
                    _scoreMultCount -= 1;
                    if(_scoreManager.scoreMultiplier > 1)
                        _scoreManager.scoreMultiplier /= 2;
                    _scoreMultTimers.RemoveAt(0);
                    goto default;
                default:
                    HideBonusInUI(type);
                    break;
            }
        }

        void Start()
        {
            this._superSpeedIcon = GameObject.Find("SuperSpeedIcon").GetComponent<Image>();
            this._slowMotionIcon = GameObject.Find("SlowMotionIcon").GetComponent<Image>();
            this._slowMotionBorder = GameObject.Find("HUDInfosPanel").GetComponent<Image>();
            this._scoreMultIcon = GameObject.Find("ScoreMultiplierIcon").GetComponent<Image>();
            this._superSpeedTimerText = GameObject.Find("SuperSpeedTimer").GetComponent<Text>();
            this._slowMotionTimerText = GameObject.Find("SlowMotionTimer").GetComponent<Text>();
            this._scoreMultTimerText = GameObject.Find("ScoreMultiplierTimer").GetComponent<Text>();
            this._superSpeedCountText = GameObject.Find("SuperSpeedCountText").GetComponent<Text>();
            this._scoreMultCountText = GameObject.Find("ScoreMultiplierCountText").GetComponent<Text>();
            this._HUDCanvas = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
            this._bonusesPanel = GameObject.Find("BonusesPanel");
            this._bonusesPanelRT = _bonusesPanel.GetComponent<RectTransform>();
            this._bonusesPanelRT.anchoredPosition += new Vector2(0f, -2*this._bonusesPanelRT.anchoredPosition.y);
            this.HidePanelWhenNoBonuses();

            this._scoreManager = Camera.main.GetComponent<ScoreManager>();
        }

        void Update()
        {
            // Update SuperSpeed timers
            for (int i = 0; i < _superSpeedTimers.Count; i++)
                _superSpeedTimers[i] -= Utils.getRealDeltaTime();
            if (_superSpeedTimers.Count > 0)
                _superSpeedTimerText.text = _superSpeedTimers[0].ToString("0.0");
            // Update SlowMotion timer
            _slowMotionTimer -= Utils.getRealDeltaTime();
            _slowMotionTimerText.text = _slowMotionTimer.ToString("0.0");
            // Update ScoreMultiplier timers
            for (int i = 0; i < _scoreMultTimers.Count; i++)
                _scoreMultTimers[i] -= Utils.getRealDeltaTime();
            if (_scoreMultTimers.Count > 0)
                _scoreMultTimerText.text = _scoreMultTimers[0].ToString("0.0");
        }

        private void ShowBonusInUI(AbstractPowerUp.PowerUpType type)
        {
            if(!_superSpeedIcon.enabled && !_slowMotionIcon.enabled && !_scoreMultIcon.enabled)
            {
                this.ShowPanelOnFirstBonus();
                Vector2 newPos = _bonusesPanelRT.anchoredPosition + new Vector2(0f, -2 * _bonusesPanelRT.anchoredPosition.y);
                iTween.ValueTo(_bonusesPanelRT.gameObject, iTween.Hash(
                    "from", _bonusesPanelRT.anchoredPosition,
                    "to", newPos,
                    "time", 0.5f,
                    "eastype", iTween.EaseType.easeOutBounce,
                    "onupdatetarget", this.gameObject,
                    "onupdate", "MoveBonusesPanel",
                    "ignoretimescale", true
                    ));
            }
            switch (type)
            {
                case AbstractPowerUp.PowerUpType.SuperSpeed:
                    _superSpeedIcon.enabled = true;
                    if (_superSpeedCount > 1)
                    {
                        _superSpeedCountText.text = "x" + _superSpeedCount.ToString();
                        _superSpeedCountText.enabled = true;
                    }
                    _superSpeedTimerText.enabled = true;
                    break;
                case AbstractPowerUp.PowerUpType.SlowMotion:
                    _slowMotionTimerText.enabled = true;
                    _slowMotionIcon.enabled = true;
                    _slowMotionBorder.enabled = true;
                    break;
                case AbstractPowerUp.PowerUpType.ScoreMultiplier:
                    _scoreMultIcon.enabled = true;
                    _scoreMultCountText.text = "x" + (Mathf.Pow(2, _scoreMultCount)).ToString();
                    _scoreMultCountText.enabled = true;
                    _scoreMultTimerText.enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void HideBonusInUI(AbstractPowerUp.PowerUpType type)
        {
            switch (type)
            {
                case AbstractPowerUp.PowerUpType.SuperSpeed:
                    if (_superSpeedCount == 0)
                    {
                        _superSpeedIcon.enabled = false;
                       _superSpeedCountText.enabled = false;
                        _superSpeedTimerText.enabled = false;
                    }
                    else if (_superSpeedCount == 1)
                        _superSpeedCountText.enabled = false;
                    else
                    {
                        _superSpeedCountText.text = "x" + _superSpeedCount.ToString();
                        _superSpeedCountText.enabled = true;
                    }
                    break;
                case AbstractPowerUp.PowerUpType.SlowMotion:
                    if (_slowMotionCount == 0)
                    {
                        _slowMotionTimerText.enabled = false;
                        _slowMotionIcon.enabled = false;
                        _slowMotionBorder.enabled = false;
                    }
                    break;
                case AbstractPowerUp.PowerUpType.ScoreMultiplier:
                    if (_scoreMultCount == 0)
                    {
                        _scoreMultIcon.enabled = false;
                        _scoreMultCountText.GetComponent<Text>().enabled = false;
                        _scoreMultTimerText.enabled = false;
                    }
                    else
                    {
                        _scoreMultCountText.text = "x" + (Mathf.Pow(2, _scoreMultCount)).ToString();
                        _scoreMultCountText.enabled = true;
                    }
                    break;
                default:
                    break;
            }
            if(!_superSpeedIcon.enabled && !_slowMotionIcon.enabled && !_scoreMultIcon.enabled)
            {
                Vector2 newPos = _bonusesPanelRT.anchoredPosition + new Vector2(0f, -2 * _bonusesPanelRT.anchoredPosition.y);
                iTween.ValueTo(_bonusesPanelRT.gameObject, iTween.Hash(
                    "from", _bonusesPanelRT.anchoredPosition,
                    "to", newPos,
                    "time", 0.5f,
                    "eastype", iTween.EaseType.easeInBounce,
                    "oncompletetarget", this.gameObject,
                    "oncomplete", "HidePanelWhenNoBonuses",
                    "onupdatetarget", this.gameObject,
                    "onupdate", "MoveBonusesPanel",
                    "ignoretimescale", true
                    ));
            }
        }

        private void ShowPanelOnFirstBonus()
        {
            _bonusesPanel.SetActive(true);
        }

        private void HidePanelWhenNoBonuses()
        {
            _bonusesPanel.SetActive(false);
        }

        private void MoveBonusesPanel(Vector2 position)
        {
            _bonusesPanelRT.anchoredPosition = position;
        }
    }
}
