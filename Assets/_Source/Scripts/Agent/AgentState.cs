namespace MiniFootball.Agent
{
    public enum AgentState
    {
        Idle,
        GetUp,
        SearchingBall,
        GoToFence,
        Patrol,
        ChaseTarget,
        ReturnToPatrol,
        InFence,
        Inactive,
    }
}