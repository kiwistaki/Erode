using Assets.Scripts.Control;
using UnityEngine;

namespace Assets.Characters.Horatio
{
    public class StrikeAnimBehaviour : StateMachineBehaviour
    {
        private bool _firstTransition = true;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this._firstTransition)
            {
                if (!animator.IsInTransition(layerIndex))
                {
                    animator.GetComponent<PlayerController>().OnAnimTransitionEvent();
                    this._firstTransition = false;
                }
            }
            else
            {
                if (animator.IsInTransition(layerIndex))
                {
                    animator.GetComponent<PlayerController>().OnAnimTransitionEvent();
                    this._firstTransition = true;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<PlayerController>().OnStrikeAnimComplete();
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
