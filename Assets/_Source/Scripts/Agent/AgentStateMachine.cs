using System;

namespace MiniFootball.Agent
{
    [Serializable]
    public class AgentStateMachine
    {
        public IState currentState { get; private set; }

        // reference to the state objects
        public IdleState IdleState;
        public RunState RunState;
        public PatrolState PatrolState;
        /*
        public WalkState walkState;
        public JumpState jumpState;
        public IdleState idleState;
        */

        // event to notify other objects of the state change
        public event Action<IState> StateChanged;

        // pass in necessary parameters into constructor 
        public AgentStateMachine(AgentController player)
        {
            this.IdleState = new IdleState(player);
            this.RunState = new RunState(player);
            this.PatrolState = new PatrolState(player);
            // create an instance for each state and pass in PlayerController
            /*
            this.walkState = new WalkState(player);
            this.jumpState = new JumpState(player);
            this.idleState = new IdleState(player);
            */
        }

        // set the starting state
        public void Initialize(IState state)
        {
            currentState = state;
            state.Enter();

            // notify other objects that state has changed
            StateChanged?.Invoke(state);
        }

        // exit this state and enter another
        public void TransitionTo(IState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }
            currentState.Exit();
            currentState = nextState;
            nextState.Enter();

            // notify other objects that state has changed
            StateChanged?.Invoke(nextState);
        }

        // allow the StateMachine to update this state
        public void Update()
        {
            currentState?.Update();
        }
    }
}