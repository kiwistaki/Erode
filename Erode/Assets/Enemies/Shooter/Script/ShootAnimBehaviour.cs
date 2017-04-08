using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Enemies.Shooter
{
    public class ShootAnimBehaviour : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<ShooterController>().AttackAnimComplete();
        }
    }
}

