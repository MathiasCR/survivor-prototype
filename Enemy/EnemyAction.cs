using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAction : MonoBehaviour
{
    [Header("Component Init")]
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Collider _weaponCollider;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private EnemyAnimator _enemyAnimator;

    [Header("Enemy State (Read Only)")]
    [SerializeField] public bool IsHit;
    [SerializeField] public bool IsDead;
    [SerializeField] public bool IsAttacking;

    private float _spawnTimer = 2f;
    private PlayerManager _playerManager;

    public bool CanAct = false;

    public bool CanMove => !IsHit && !IsDead && !IsAttacking;

    public bool CanAttack => !IsHit && !IsDead && !IsAttacking;

    private void Awake()
    {
        _playerManager = GameManager.Instance.GetPlayerManager();
        _enemy.OnEnemyHit += OnEnemyHit;
        _enemy.OnEnemyDeath += OnEnemyDeath;
    }

    private void Start()
    {
        _navMeshAgent.speed = _enemy.MoveSpeed;
        StartCoroutine(WaitSpawn());
    }

    private void Update()
    {
        CanAct = !_playerManager.IsInMenu;
    }

    private void FixedUpdate()
    {
        if (!IsDead)
        {
            Vector3 playerPosition = _playerManager.transform.position;
            transform.LookAt(playerPosition);

            if ((_playerManager.PlayerLife <= 0) || !CanAct || !CanMove)
            {
                _navMeshAgent.isStopped = true;
                _enemyAnimator.EnemyRunningAnimation(false);
                return;
            }
            else if (CanAct)
            {
                ControlEnemy(playerPosition);
            }
        }
    }

    private void ControlEnemy(Vector3 playerPosition)
    {
        if (CanMove)
        {
            // Check if the position of the enemy and player are close enough.
            if (Vector3.Distance(transform.position, playerPosition) > _enemy.AttackRange)
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.destination = playerPosition;
                _enemyAnimator.EnemyRunningAnimation(true);
            }
            else if (CanAttack)
            {
                Attack();
            }
            else
            {
                _navMeshAgent.isStopped = true;
                _enemyAnimator.EnemyRunningAnimation(false);
            }
        }
    }

    private void Attack()
    {
        IsAttacking = true;
        _navMeshAgent.isStopped = true;
        _enemyAnimator.EnemyRunningAnimation(false);

        _enemyAnimator.EnemyAttackingAnimation();
        _enemyAnimator.OnAttackAnimationCollide += WeaponCollide;
        _enemyAnimator.OnAttackAnimationEnded += EndAttack;
    }

    private void WeaponCollide()
    {
        _weaponCollider.enabled = true;
    }

    private void EndAttack()
    {
        _weaponCollider.enabled = false;
        StartCoroutine(AttackCooldown());
    }

    private void OnEnemyHit(float stunTimer)
    {
        IsHit = true;
        _enemyAnimator.EnemyHitAnimation(stunTimer);
        _enemyAnimator.OnHitAnimationEnded += OnHitAnimationEnded;
    }

    private void OnHitAnimationEnded()
    {
        IsHit = false;
        _enemyAnimator.OnHitAnimationEnded -= OnHitAnimationEnded;
    }

    private void OnEnemyDeath()
    {
        IsDead = true;
        _navMeshAgent.enabled = true;
        _weaponCollider.enabled = false;
        _enemyAnimator.EnemyDeadAnimation();
        _enemyAnimator.OnDeadAnimationEnded += OnDeadAnimationEnded;
    }

    private void OnDeadAnimationEnded()
    {
        StartCoroutine(WaitRespawnTimer());
        _enemyAnimator.OnDeadAnimationEnded -= OnDeadAnimationEnded;
    }

    private IEnumerator WaitSpawn()
    {
        yield return new WaitForSeconds(_spawnTimer);
        CanAct = true;
    }

    protected virtual IEnumerator WaitRespawnTimer()
    {
        Vector3 position = _enemyAnimator.gameObject.transform.position;

        while (position.y > 0.4f)
        {
            position -= new Vector3(0f, 0.001f, 0f);

            _enemyAnimator.gameObject.transform.position = position;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_enemy.AttackSpeed);
        IsAttacking = false;
    }
}
