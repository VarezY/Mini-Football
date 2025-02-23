namespace MiniFootball.Agent
{
    public interface IState
    {
        void Enter();

        void Update();

        void Exit();
        
    }
}