using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyManager : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] private EnemyGUI _enemyGUI;
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private EnemyAction _enemyAction;
    [SerializeField] private GameObject _explodedHead;
    [SerializeField] private SkinnedMeshRenderer _head;
    [SerializeField] private GameObject _equippedWeapon;
    [SerializeField] private SkinnedMeshRenderer _hands;
    [SerializeField] private BoxCollider _enemyCollider;
    [SerializeField] private ParticleSystem _bloodPoolPS;
    [SerializeField] private EnemyAnimator _enemyAnimator;
    [SerializeField] private ParticleSystem _bloodSplatterUpPS;
    [SerializeField] private List<SkinnedMeshRenderer> _headEquipments;

    [Header("Base Enemy Settings (ReadOnly)")]
    [SerializeField] public int EnemyLife;
    [SerializeField] public EnemyType EnemyType;
    [SerializeField] public EnemyTier EnemyTier;
    [SerializeField] private int _currentEnemyLife;

    [Header("Enemy Attack Settings")]
    [SerializeField] public float AttackRange;
    [SerializeField] public float AttackSpeed;
    [SerializeField] public float AimingSpeed;

    [Header("Movement Enemy Settings")]
    [SerializeField] public float MoveSpeed;

    [Header("Item Drop Enemy Settings")]
    [SerializeField] public int ItemDropRate;
    [SerializeField] public int CommonDropRate;
    [SerializeField] public int RareDropRate;
    [SerializeField] public int EpicDropRate;
    [SerializeField] public int LegendaryDropRate;

    private WaitForSeconds _waitForSeconds = new WaitForSeconds(0.1f);

    protected virtual void Awake()
    {
        EnemyLife = _enemyData.EnemyLife;
        EnemyType = _enemyData.EnemyType;
        EnemyTier = _enemyData.EnemyTier;
        MoveSpeed = _enemyData.MoveSpeed;
        AttackSpeed = _enemyData.AttackSpeed;
        AttackRange = _enemyData.AttackRange;
        AimingSpeed = _enemyData.AimingSpeed;
        ItemDropRate = _enemyData.ItemDropRate;
        CommonDropRate = _enemyData.CommonDropRate;
        RareDropRate = _enemyData.RareDropRate;
        EpicDropRate = _enemyData.EpicDropRate;
        LegendaryDropRate = _enemyData.LegendaryDropRate;
    }

    protected virtual void OnEnable()
    {
        _hands.enabled = false;
        _head.enabled = true;
        foreach (SkinnedMeshRenderer equipment in _headEquipments)
        {
            equipment.enabled = true;
        }

        //_enemyAction.SetDead(false);
        _enemyCollider.enabled = true;
        _equippedWeapon.SetActive(true);
        _enemyAnimator.gameObject.transform.position = transform.position;
        _currentEnemyLife = _enemyData.EnemyLife;
        _enemyGUI.SetHealthBar(_enemyData.EnemyLife);
    }

    public virtual void OnEnemyHit(int damage, float stunTimer, bool isHeadShot)
    {
        _currentEnemyLife -= damage;
        if (_currentEnemyLife <= 0)
        {
            CleanEnemyOnDeath();
            //_enemyAction.SetDead(true);

            GameManager.Instance.OnEnemyDead(this);

            if (isHeadShot)
            {
                ExplodeEnemyHead();
                _enemyAnimator.EnemyOverkillAnimation();
                _enemyAnimator.OnDeadAnimationEnded += OnDeadAnimationEnded;
            }
            else
            {
                _enemyAnimator.EnemyDeadAnimation();
                _enemyAnimator.OnDeadAnimationEnded += OnDeadAnimationEnded;
            }
        }
        else
        {
            //_enemyAction.SetHit(true);
            _enemyGUI.UpdateHealthBar(_currentEnemyLife);
            StartCoroutine(WaitStunTimer(stunTimer));
            if (isHeadShot)
            {
                _enemyAnimator.EnemyHeadShotAnimation();
            }
            else
            {
                _enemyAnimator.EnemyHitAnimation(stunTimer);
            }
        }
    }

    protected virtual void ExplodeEnemyHead()
    {
        _head.enabled = false;
        foreach (SkinnedMeshRenderer equipment in _headEquipments)
        {
            equipment.enabled = false;
        }

        _bloodSplatterUpPS.Play();
        GameObject explodedHead = Instantiate(_explodedHead, _head.transform.position + new Vector3(0f, 2f, 0f), _head.transform.rotation);
        foreach (Transform t in explodedHead.transform)
        {
            if (t.TryGetComponent(out Rigidbody rb))
            {
                rb.AddRelativeForce(new Vector3(Random.Range(-2f, 2f), 1.5f, 2f), ForceMode.Impulse);

                StartCoroutine(DespawnHeadPieces(t));
            }
        }
    }

    protected virtual IEnumerator DespawnHeadPieces(Transform t)
    {
        Vector3 newScale = t.localScale;

        while (newScale.x > 0.00f)
        {
            newScale -= new Vector3(0.01f, 0.01f, 0.01f);

            t.localScale = newScale;
            yield return _waitForSeconds;
        }

        Destroy(t.gameObject);
    }

    protected virtual void CleanEnemyOnDeath()
    {
        _hands.enabled = true;
        _enemyCollider.enabled = false;
        _enemyGUI.HideHealthBar();
        _equippedWeapon.SetActive(false);
    }

    protected virtual void OnDeadAnimationEnded()
    {
        _enemyAnimator.OnDeadAnimationEnded -= OnDeadAnimationEnded;
        _bloodPoolPS.Play();
        StartCoroutine(WaitRespawnTimer());
    }

    protected virtual IEnumerator WaitStunTimer(float stunTimer)
    {
        yield return new WaitForSeconds(stunTimer);
        //_enemyAction.SetHit(false);
    }

    protected virtual IEnumerator WaitRespawnTimer()
    {
        Vector3 position = _enemyAnimator.gameObject.transform.position;

        while (position.y > 0.4f)
        {
            position -= new Vector3(0f, 0.001f, 0f);

            _enemyAnimator.gameObject.transform.position = position;
            yield return _waitForSeconds;
        }

        Destroy(gameObject);
    }
}
