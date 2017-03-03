namespace Assets.Scripts.StateMachine
{
    public abstract class AbstractState
    {
        public abstract void Enter();

        public abstract void OnStateUpdate();

        public abstract void Exit();
    }
}
