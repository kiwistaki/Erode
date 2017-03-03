using Assets.Obstacles.Asteroide;
using Assets.Scripts.HexGridGenerator;
using MirzaBeig.ParticleSystems;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public class HammerController : MonoBehaviour
    {
        public enum HammerType
        {
            Undefined = -1,
            Quick,
            Charged,
            Spin,
            Disabled
        }

        public PlayerController PlayerController;
        public GameObject ChargedStrikeOnTileVfx;

        public HammerType StrikeHammerType { get; set; }

        void Awake()
        {
            this.StrikeHammerType = HammerType.Undefined;
        }

        public void OnAsteroidCollision(GameObject asteroid)
        {
            var ast = asteroid.GetComponent<AsteroideController>();
            GameManager.destroyScore += 3;
            switch (this.StrikeHammerType)
            {
                case HammerType.Undefined:
                    throw new UnityException("HammerController::OnAsteroidCollision: Usage of undefined enum");

                case HammerType.Quick:
                    ast.AsteroidVelocity = this.PlayerController.transform.forward * ast.AsteroidVelocity.magnitude;
                    ast.HitType = HammerType.Quick;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Charged:
                    ast.AsteroidVelocity = this.PlayerController.transform.forward * this.PlayerController.MovementSpeed * 2.0f;
                    ast.HitType = HammerType.Charged;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Spin:
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
            var hunt = hunter.GetComponent<HunterAI>();
            switch (this.StrikeHammerType)
            {
                case HammerType.Undefined:
                    throw new UnityException("HammerController::OnHunterCollision: Usage of undefined enum");

                case HammerType.Quick:
                    hunt.HitPoint -= 1;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Charged:
                    hunt.HitPoint -= 1;
                    this.PlayerController.PlayHitEffect(this.transform.position);
                    break;

                case HammerType.Spin:
                    //hunt.HitPoint -= 1;
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

        public void OnTileCollision(GameObject tile)
        {
            var t = tile.GetComponent<Tile>();
            if(this.StrikeHammerType == HammerType.Charged)
            {
                // Explosion effect
                var pos = tile.transform.position;
                pos.y = 0.5f;
                var badaboom = Instantiate(this.ChargedStrikeOnTileVfx, pos, Quaternion.identity).GetComponent<ParticleSystems>();
                badaboom.setPlaybackSpeed(0.5f);
                badaboom.simulate(.1f);
                badaboom.play();
                // Delete tiles
                var neighbors = t.Neighbours;
                foreach (Tile n in neighbors)
                {
                    n.Hp = 0;
                    n.Hide();
                }
                t.Hp = 0;
                t.Hide();

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Tile")
            {
                OnTileCollision(other.gameObject);
            }
        }
    }
}