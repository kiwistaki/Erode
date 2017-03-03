namespace Assets.Scripts.StateMachine
{
    public abstract class AbstractStateMachine
    {
        protected AbstractState _currentState = null;

        protected AbstractStateMachine(AbstractState state)
        {
            this._currentState = state;
            this._currentState.Enter();
        }
        
        public void UpdateStateMachine()
        {
            this._currentState.OnStateUpdate();
        }

        protected void ChangeState(AbstractState state)
        {
            //this._stateQueue.Enqueue(state);
            this._currentState.Exit();
            this._currentState = state;
            this._currentState.Enter();
        }
    }
}
