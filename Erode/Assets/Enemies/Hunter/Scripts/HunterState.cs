using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{

    public abstract class HunterState : AbstractState
    {
        protected readonly HunterController _hunterController = null;
        protected readonly object _args = null;

        protected HunterState(HunterController hunter, object args)
        {
            this._hunterController = hunter;
            this._args = args;
        }

        public abstract HunterCharacterStateMachine.HunterState GetStateType();
    }
}