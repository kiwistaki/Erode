using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{

    public abstract class HunterState : AbstractState
    {
        protected readonly HunterAI _hunterAI = null;
        protected readonly object _args = null;

        protected HunterState(HunterAI hunter, object args)
        {
            this._hunterAI = hunter;
            this._args = args;
        }

        public abstract HunterCharacterStateMachine.HunterState GetStateType();
    }
}