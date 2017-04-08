using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Control
{
    public abstract class ShooterState : AbstractState
    {
        protected readonly ShooterController _shooterController = null;
        protected readonly object _args = null;

        protected ShooterState(ShooterController shooter, object args)
        {
            this._shooterController = shooter;
            this._args = args;
        }

        public abstract ShooterCharacterStateMachine.ShooterState GetStateType();
    }
}

