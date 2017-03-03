using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Control;
using UnityEngine;

namespace Assets.Enemies.Hunter
{
    public class DieAnimBehaviour : StateMachineBehaviour
    { 
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<HunterAI>().DefaultDieAnimComplete();
        }
    }
}

