using Assets.Scripts.HexGridGenerator;
using Assets.Scripts.iTween;
using Assets.Scripts.Spawners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Level
{
    class LevelManager : MonoBehaviour
    {
        public GameObject Spawners;

        private ScoreManager _scoreManager;
        private Text _levelText;

        private Dictionary<string, AbstractSpawner> _spawners = new Dictionary<string, AbstractSpawner>();
        private int _scoreToNextLevel = 1000000;



        public enum LevelNumber
        {
            Level0 = -1,
            Level1, Level2, Level3,
            Level4, Level5, Level6,
            Level7, Level8, Level9,
            Level10, Level11, Level12,
            Level13, Level14, Level15,
            testLevel
        }
        private LevelNumber _currentLevel = LevelNumber.Level0;
        private string[] _levelString = {
            "level1", "level2", "level3",
            "level4","level5","level6",
            "level7","level8","level9",
            "level10","level11","level12",
            "level13","level14","level15",
            "testLevel"
        };

        private void Awake()
        {
            _scoreManager = GameObject.Find("MainCamera").GetComponent<ScoreManager>();
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();

            _spawners.Add("asteroid", Spawners.GetComponent<Spawners.AsteroidSpawner>());
            _spawners.Add("blackhole", Spawners.GetComponent<Spawners.BlackholeSpawner>());
            _spawners.Add("charger", Spawners.GetComponent<Spawners.ChargerSpawner> ());
            _spawners.Add("comet", Spawners.GetComponent<Spawners.CometSpawner>());
            _spawners.Add("emp", Spawners.GetComponent<Spawners.EMPSpawner>());
            _spawners.Add("hunter", Spawners.GetComponent<Spawners.HunterSpawner>());
            _spawners.Add("onyx", Spawners.GetComponent<Spawners.OnyxSpawner>());
            _spawners.Add("shooter", Spawners.GetComponent<Spawners.ShooterSpawner>());
            _spawners.Add("solarwind", Spawners.GetComponent<Spawners.SolarwindSpawner>());
            _spawners.Add("powerups", Spawners.GetComponent<Spawners.PowerUpSpawner>());
        }

        private void Update()
        {
            if (_scoreManager.getLevelScore() > _scoreToNextLevel)
                LoadLevel(++_currentLevel);
        }

        public void StartGame()
        {
            if (_currentLevel == LevelNumber.Level0) 
            {
                _currentLevel++;
            }
                LoadLevel(_currentLevel);
        }

        public LevelNumber getLevel() 
        {
            return _currentLevel;
        }

        public void SetLevel(LevelNumber lvl) 
        {
            _currentLevel = lvl;
        }

        public void IncreaseLevel() 
        {
            _currentLevel++;
            Debug.Log( "Current Level: " + _currentLevel);
        }

        public void DecreaseLevel()
        {
            _currentLevel--;
            Debug.Log("Current Level: " + _currentLevel);
        }

        public void LoadLevel(LevelNumber level)
        {
            // Unload last level
            foreach (var pair in _spawners)
                pair.Value.DeactivateSpawner();

            string levelName = _levelString[(int)level];
            _scoreManager.ChangeLevel(levelName);
            GameObject parent = new GameObject(levelName);
            // Load the requested level
            using (var xmlReader = new StreamReader(Directory.GetCurrentDirectory() + "\\Assets\\Scripts\\Level\\LevelConfigs.xml"))
            {
                var doc = XDocument.Load(xmlReader);
                XNamespace nonamespace = XNamespace.None;
                var lvlQuery = (from c in doc.Descendants(_levelString[(int)level]) select c).First<XElement>();
                _scoreToNextLevel = Convert.ToInt32(lvlQuery.Attribute("score").Value);
                Grid.inst.HexErodeRate = (float)Convert.ToDouble(lvlQuery.Attribute("erodeRate").Value);
                var xmlSpawners = doc.Descendants(nonamespace + levelName);
                foreach(var item in xmlSpawners.Elements<XElement>())
                {
                    AbstractSpawner spawner;
                    _spawners.TryGetValue(item.Name.LocalName, out spawner);
                    int intensity = Convert.ToInt32(item.Attribute(XName.Get("intensity")).Value), 
                        baseNumber = Convert.ToInt32(item.Attribute(XName.Get("baseNumber")).Value), 
                        baseTime = Convert.ToInt32(item.Attribute(XName.Get("baseTime")).Value);
                    if (item.Name.LocalName == "powerups")
                    {
                        bool IsSpeedPresent = Convert.ToBoolean(item.Attribute(XName.Get("speed")).Value),
                            IsSlowPresent = Convert.ToBoolean(item.Attribute(XName.Get("slow")).Value),
                            IsScorePresent = Convert.ToBoolean(item.Attribute(XName.Get("score")).Value);
                        if (IsSpeedPresent) ((Spawners.PowerUpSpawner)spawner).EnablePowerUp(PowerUpSpawner.PowerUpType.SuperSpeed);
                        if (IsSlowPresent) ((Spawners.PowerUpSpawner)spawner).EnablePowerUp(PowerUpSpawner.PowerUpType.SlowMotion);
                        if (IsScorePresent) ((Spawners.PowerUpSpawner)spawner).EnablePowerUp(PowerUpSpawner.PowerUpType.ScoreMultiplier);
                    }
                    spawner.Initialize(intensity, baseNumber, baseTime, parent);
                    spawner.ActivateSpawner();
                }
            }

            // Show current level in UI
            this._levelText.GetComponent<Text>().text = "Level " + (int)(_currentLevel + 1);
            this._levelText.color = new Color(1f, 1f, 1f, 0f);
            iTween.iTween.ValueTo(this._levelText.gameObject, iTween.iTween.Hash(
            "name", "Appear",
            "from", new Color(1f, 1f, 1f, 0f),
            "to", new Color(1f,1f,1f,1f),
            "time", 2f,
            "onupdatetarget", this.gameObject,
            "onupdate", "UpdateAppear",
            "oncompletetarget", this.gameObject,
            "oncomplete", "OnCompleteAppear",
            "ignoretimescale", true
            ));
        }

        private void UpdateAppear(Color color)
        {
            this._levelText.color = color;
        }

        private void OnCompleteAppear()
        {
            iTween.iTween.ValueTo(this._levelText.gameObject, iTween.iTween.Hash(
            "name", "Disappear",
            "from", new Color(1f, 1f, 1f, 1f),
            "to", new Color(1f, 1f, 1f, 0f),
            "time", 2f,
            "onupdatetarget", this.gameObject,
            "onupdate", "UpdateDisappear",
            "ignoretimescale", true
            ));
        }
        
        private void UpdateDisappear(Color color)
        {
            this._levelText.color = color;
        }
    }
}
