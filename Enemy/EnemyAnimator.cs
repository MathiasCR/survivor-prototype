using System;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Animator _animator;

    public event Action OnHitAnimationEnded;
    public event Action OnDeadAnimationEnded;
    public event Action OnAttackAnimationEnded;
    public event Action OnAttackAnimationCollide;

    public void EnemyRunningAnimation(bool isMoving)
    {
        _animator.SetBool("IsRunning", isMoving);
    }

    public void EnemyAttackingAnimation()
    {
        _animator.SetTrigger("Attack");
    }

    public void AttackAnimationCollide()
    {
        OnAttackAnimationCollide?.Invoke();
    }

    public void AttackAnimationEnded()
    {
        OnAttackAnimationEnded?.Invoke();
    }

    public void EnemyHitAnimation(float stunTimer)
    {
        _animator.SetFloat("HitTimer", stunTimer);
        _animator.SetTrigger("Hit");
    }

    public void HitAnimationEnded()
    {
        OnHitAnimationEnded?.Invoke();
    }

    public void EnemyHeadShotAnimation()
    {
        _animator.SetTrigger("HeadShot");
    }

    public void EnemyDeadAnimation()
    {
        _animator.SetTrigger("Dead");
    }

    public void EnemyOverkillAnimation()
    {
        _animator.SetTrigger("Overkilled");
    }

    public void DeadAnimationEnded()
    {
        OnDeadAnimationEnded?.Invoke();
    }
}
