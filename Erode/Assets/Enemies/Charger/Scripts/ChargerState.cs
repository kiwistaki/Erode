using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{

    public abstract class ChargerState : AbstractState
    {
        protected readonly ChargerController _chargerController = null;
        protected readonly object _args = null;

        protected ChargerState(ChargerController charger, object args)
        {
            this._chargerController = charger;
            this._args = args;
        }

        public abstract ChargerStateMachine.ChargerState GetStateType();
    }
}
