using System.Collections;
using UnityEngine;

public class TestEnemy : EnemyBase
{
    Rigidbody rb;

    Transform playerTransform;

    private void Awake()
    {
        InitNavMesh();
        TryGetComponent(out rb);
        rb.isKinematic = true;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(EnemyNavMeshAgent.isActiveAndEnabled)
            EnemyNavMeshAgent.SetDestination(playerTransform.position);
    }

    public override void TakeDamage(int damage, Vector3 hitPoint)
    {
        EnemyNavMeshAgent.isStopped = true;
        EnemyNavMeshAgent.enabled = false;
        rb.isKinematic = false;

        base.TakeDamage(damage, hitPoint);

        Vector3 playerPos = playerTransform.position;
        Vector3 myPos = transform.position;
        playerPos.y = myPos.y = 0;
        Vector3 knockbackDir = myPos - playerPos;
        rb.AddForce(knockbackDir * 120f, ForceMode.Impulse);
        StartCoroutine(EnableIsKinematic());
    }

    private IEnumerator EnableIsKinematic()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);

        while(rb.linearVelocity.magnitude > 0.01f)
        {
            yield return null;
        }

        rb.isKinematic = true;
        EnemyNavMeshAgent.enabled = true;
        EnemyNavMeshAgent.isStopped = false;
    }
}
