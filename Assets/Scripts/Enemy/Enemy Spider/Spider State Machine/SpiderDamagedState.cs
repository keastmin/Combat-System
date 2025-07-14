using UnityEngine;

public class SpiderDamagedState : IState
{
    private EnemySpider _enemySpider;

    private float _damagedTime = 0f;

    public SpiderDamagedState(EnemySpider enemySpider)
    {
        this._enemySpider = enemySpider;
    }

    public void Enter()
    {
        _enemySpider.EnemyNavMeshAgent.isStopped = true;
        _damagedTime = 0f;

        _enemySpider.SpiderAnimator.SetTrigger("IsDamaged");

        _enemySpider.SpiderRigidbody.MovePosition(_enemySpider.SpiderRigidbody.position - (_enemySpider.transform.forward * _enemySpider.KnockbackForce));

        //Vector3 hitPoint = _enemySpider.HitPoint;
        //Vector3 enemyPos = _enemySpider.transform.position;
        //hitPoint.y = 0f;
        //enemyPos.y = 0f;
        //Vector3 knockbackDir = enemyPos - hitPoint;
        //Debug.Log(knockbackDir.normalized);
        //_enemySpider.SpiderRigidbody.AddForce(knockbackDir.normalized * _enemySpider.KnockbackForce, ForceMode.Impulse);
    }

    public void Execute()
    {
        TransitionTo();
    }

    public void FixedExecute()
    {
        
    }

    public void AnimatorMove()
    {

    }

    public void Exit()
    {
        _enemySpider.EnemyNavMeshAgent.isStopped = false;
        _enemySpider.ClearDamage();
    }

    private void TransitionTo()
    {
        _damagedTime += Time.deltaTime;
        if(_damagedTime >= _enemySpider.DamagedTime)
        {
            _enemySpider.StateMachine.Transition(_enemySpider.StateMachine.IdleState);
        }
    }
}
