using System.Collections.Generic;
using Assets.Obstacles.Asteroide;
using Assets.Scripts.HexGridGenerator;
using MirzaBeig.ParticleSystems;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Control
{
    public class HammerController : MonoBehaviour
    {
        public enum HammerType
        {
            Undefined = -1,
            Quick,
            ChargedLow,
            ChargedMed,
            ChargedMax,
            Spin,
            Disabled
        }

        public PlayerController PlayerController;
        public GameObject ChargedStrikeOnTileVfx;
        private ScoreManager _scoreManager;
        [Range(2, 100)]
        public int LowWaveRadius = 10;
        [Range(2, 100)]
        public int MedWaveRadius = 20;
        [Range(2, 100)]
        public int HighWaveRadius = 30;

        public HammerType StrikeHammerType { get; set; }

        void Awake()
        {
            this.StrikeHammerType = HammerType.Undefined;
        }

        public void OnAsteroidCollision(GameObject asteroid)
        {
            var ast = asteroid.GetComponent<AsteroideController>();
            switch (this.StrikeHammerType)
            {
                case HammerType.Undefined:
                    throw new UnityException("HammerController::OnAsteroidCollision: Usage of undefined enum");

                case HammerType.Quick:

                    //_scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Asteroid, ast.transform.parent.name);
                    //_scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Asteroid, ast.transform.position);
                    ast.AsteroidVelocity = this.PlayerController.transform.forward * ast.AsteroidVelocity.magnitude;
                    ast.HitType = HammerType.Quick;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedLow:
                    //_scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Asteroid, ast.transform.parent.name);
                   // _scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Asteroid, ast.transform.position);
                    ast.AsteroidVelocity = this.PlayerController.transform.forward * this.PlayerController.MovementSpeed * 1.3f;
                    ast.HitType = HammerType.ChargedLow;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMed:
                    //_scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Asteroid, ast.transform.parent.name);
                    //_scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Asteroid, ast.transform.position);
                    ast.AsteroidVelocity = this.PlayerController.transform.forward * this.PlayerController.MovementSpeed * 1.6f;
                    ast.HitType = HammerType.ChargedLow;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMax:
                    //_scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Asteroid, ast.transform.parent.name);
                    //_scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Asteroid, ast.transform.position);
                    ast.AsteroidVelocity = this.PlayerController.transform.forward * this.PlayerController.MovementSpeed * 2.0f;
                    ast.HitType = HammerType.ChargedLow;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Spin:
                    //_scoreManager.IncrementDestroyScore(ScoreManager.ScoreType.Asteroid, ast.transform.parent.name);
                    //_scoreManager.showScoreOnDestroy(ScoreManager.ScoreType.Asteroid, ast.transform.position);
                    ast.AsteroidVelocity = this.PlayerController.transform.forward * ast.AsteroidVelocity.magnitude;
                    ast.HitType = HammerType.Spin;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    this.PlayerController.ChangeState(PlayerCharacterStateMachine.PlayerStates.Knockbacked, asteroid);
                    break;

                case HammerType.Disabled:
                    //do nothing
                    break;

                default:
                    throw new UnityException("HammerController::OnAsteroidCollision: " + this.StrikeHammerType.ToString() + " IS NOT IMPLEMENTED");
            }
        }

        public void OnHunterCollision(GameObject hunter)
        {
            var hunt = hunter.GetComponent<HunterController>();
            switch (this.StrikeHammerType)
            {
                case HammerType.Undefined:
                    throw new UnityException("HammerController::OnHunterCollision: Usage of undefined enum");

                case HammerType.Quick:
                    hunt.HitPoint -= hunt.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMed:
                    //this is a C# fallthrough
                    goto case HammerType.ChargedLow;

                case HammerType.ChargedMax:
                    //this is a C# fallthrough
                    goto case HammerType.ChargedLow;

                case HammerType.ChargedLow:
                    hunt.HitPoint -= hunt.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Spin:
                    hunt.HitPoint -= 1;
                    if(hunt.HitPoint > 0)
                        hunt.OnAsteroidCollision(this.PlayerController.gameObject);
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Disabled:
                    //do nothing
                    break;

                default:
                    throw new UnityException("HammerController::OnHunterCollision: " + this.StrikeHammerType.ToString() + " IS NOT IMPLEMENTED");
            }
        }

        public void OnShooterCollision(GameObject shooter)
        {
            var shoot = shooter.GetComponent<ShooterController>();
            switch (this.StrikeHammerType)
            {
                case HammerType.Undefined:
                    throw new UnityException("HammerController::OnShooterCollision: Usage of undefined enum");

                case HammerType.Quick:
                    shoot.HitPoint -= shoot.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMed:
                    //this is a C# fallthrough
                    goto case HammerType.ChargedLow;

                case HammerType.ChargedMax:
                    //this is a C# fallthrough
                    goto case HammerType.ChargedLow;

                case HammerType.ChargedLow:
                    shoot.HitPoint -= shoot.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Spin:
                    shoot.HitPoint -= 1;
                    if (shoot.HitPoint > 0)
                        shoot.OnAsteroidCollision(this.PlayerController.gameObject);
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Disabled:
                    //do nothing
                    break;

                default:
                    throw new UnityException("HammerController::OnShooterCollision: " + this.StrikeHammerType.ToString() + " IS NOT IMPLEMENTED");
            }
        }

        public void OnChargerCollision(GameObject charger)
        {
            var charg = charger.GetComponent<ChargerController>();
            switch (this.StrikeHammerType)
            {
                case HammerType.Undefined:
                    throw new UnityException("HammerController::OnHunterCollision: Usage of undefined enum");

                case HammerType.Quick:
                    charg.HitPoint -= charg.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedLow:
                    charg.HitPoint -= charg.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMed:
                    charg.HitPoint -= charg.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMax:
                    charg.HitPoint -= charg.HitPoint;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Spin:
                    charg.HitPoint -= 1;
                    charg.OnAsteroidCollision(this.PlayerController.gameObject);
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Disabled:
                    //do nothing
                    break;

                default:
                    throw new UnityException("HammerController::OnChargerCollision: " + this.StrikeHammerType.ToString() + " IS NOT IMPLEMENTED");
            }
        }

        public void OnChargerCollisionRunning(GameObject charger)
        {
            var charg = charger.GetComponent<ChargerController>();
            switch (this.StrikeHammerType)
            {
                case HammerType.Undefined:
                    throw new UnityException("HammerController::OnHunterCollision: Usage of undefined enum");

                case HammerType.Quick:
                    //Ne fait rien
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedLow:
                    charg.HitPoint -= 1;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMed:
                    charg.HitPoint -= 2;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.ChargedMax:
                    charg.HitPoint -= 3;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Spin:
                    charg.DefaultCollision(this.PlayerController.gameObject);
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Disabled:
                    //do nothing
                    break;

                default:
                    throw new UnityException("HammerController::OnHunterCollision: " + this.StrikeHammerType.ToString() + " IS NOT IMPLEMENTED");
            }
        }
        public void OnTileCollision(GameObject tile)
        {
            var t = tile.GetComponent<Tile>();
            switch (this.StrikeHammerType)
            {
                case HammerType.ChargedMed:
                    if (!Grid.inst.IsGridWaving)
                    {
                        this.StartTileWave(t, this.MedWaveRadius);
                        Grid.inst.IsGridWaving = true;
                    }
                    break;

                case HammerType.ChargedMax:
                    if (!Grid.inst.IsGridWaving)
                    {
                        this.StartTileWave(t, this.HighWaveRadius);
                        Grid.inst.IsGridWaving = true;
                    }

                    // Explosion effect
                    var pos = tile.transform.position;
                    pos.y = 0.5f;
                    var badaboom = Instantiate(this.ChargedStrikeOnTileVfx, pos + Vector3.up, Quaternion.identity).GetComponent<ParticleSystems>();
                    badaboom.setPlaybackSpeed(0.5f);
                    badaboom.simulate(.1f);
                    badaboom.play();
                    // Delete tiles
                    foreach (var n in t.Neighbours)
                    {
                        n.ErodeKill();
                        n.Hide();
                    }
                    t.ErodeKill();
                    t.Hide();
                    break;

                case HammerType.ChargedLow:
                    if (!Grid.inst.IsGridWaving)
                    {
                        this.StartTileWave(t, this.LowWaveRadius);
                        Grid.inst.IsGridWaving = true;
                    }
                    break;
            }
            this.StrikeHammerType = HammerType.Disabled;
        }

        private void StartTileWave(Tile t, int waveRadius)
        {
            // Initialize the "wave"
            List<List<Tile>> concentricCircles = new List<List<Tile>>();
            for (int i = 0; i < waveRadius; i++)
                concentricCircles.Add(new List<Tile>());
            // Find all the concentric circles from the center
            foreach (KeyValuePair<string, Tile> pair in Grid.inst.Tiles)
            {
                int distance = Grid.inst.Distance(pair.Value, t);
                if (distance < waveRadius)
                    concentricCircles[distance].Add(pair.Value);
            }
            // Set all the iTweens
            float delay = 0.0f;
            foreach (List<Tile> circle in concentricCircles)
            {
                foreach (Tile tile in circle)
                {
                    iTween.iTween.PunchPosition(tile.gameObject, iTween.iTween.Hash
                       ("name", "Wave"
                           , "time", 0.5f
                           , "delay", delay
                           , "onstarttarget", tile.gameObject
                           , "onstart", "ToWavingState"
                           , "onupdatetarget", tile.gameObject
                           , "onupdate", "UpdateYAxis"
                           , "oncompletetarget", tile.gameObject
                           , "oncomplete", "ResetTransform"
                           , "ignoretimescale", false
                       ));
                }
                delay += 0.05f;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Tile")
            {
                this.OnTileCollision(other.gameObject);
            }
        }
    }
}