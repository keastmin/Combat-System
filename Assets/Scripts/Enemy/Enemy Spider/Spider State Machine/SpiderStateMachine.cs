using UnityEngine;

public class SpiderStateMachine
{
    public SpiderIdleState IdleState;
    public SpiderPatrolState PatrolState;
    public SpiderChaseState ChaseState;
    public SpiderDamagedState DamagedState;

    private IState _currentState;

    private EnemySpider _enemySpider;

    private float _damagedCoolTime = 0f;

    public SpiderStateMachine(EnemySpider enemySpider)
    {
        _enemySpider = enemySpider;
        IdleState = new SpiderIdleState(enemySpider);
        PatrolState = new SpiderPatrolState(enemySpider);
        ChaseState = new SpiderChaseState(enemySpider);
        DamagedState = new SpiderDamagedState(enemySpider);
    }

    public void InitState(IState initState)
    {
        _currentState = initState;
        _currentState.Enter();
    }

    public void Transition(IState nextState)
    {
        _currentState.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }

    public void Execute()
    {
        if(_damagedCoolTime < _enemySpider.DamagedCooldown)
        {
            _damagedCoolTime += Time.deltaTime;
        }

        if (_enemySpider.IsDamaged && _damagedCoolTime >= _enemySpider.DamagedCooldown)
        {
            _damagedCoolTime = 0f;
            Transition(DamagedState);
            return;
        }

        _currentState.Execute();
    }
}
