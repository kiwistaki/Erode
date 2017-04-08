using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Enemies.Charger
{
    public class ChargerShoutAnimBehaviour : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<ChargerController>().ShoutAnimComplete(animator.transform.gameObject);
        }
    }
}
