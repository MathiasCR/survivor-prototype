using System.Collections;
using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] private Collider _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _bulletHud;
    [SerializeField] private float _bulletVelocity;
    [SerializeField] private float _bulletLifeTime;
    [SerializeField] private GameObject _bulletTrail;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private TextMeshProUGUI _bulletDamageText;
    [SerializeField] private TextMeshProUGUI _bulletHeadShotDamageText;
    [SerializeField] private BloodSplatterParticle _classicBulletBloodSplatter;

    [Header("Bullet Settings (Read Only)")]
    public bool IsHeadShot;
    public float StunTimer;
    public int BulletDamage;
    public LayerMask LayerToIgnore;

    private float _currentVelocity;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        _trailRenderer.Clear();
        _currentVelocity = _bulletVelocity;
        _collider.enabled = true;
        _animator.SetTrigger("BulletSpawn");
        StartCoroutine(waitBeforeReturnBulletToPool());
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * _currentVelocity * Time.fixedDeltaTime;

        Vector3 rotation = transform.position - Camera.main.transform.position;
        _bulletHud.transform.rotation = Quaternion.LookRotation(rotation);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerToIgnore.value) return;

        HideBullet();
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerManager player = GameManager.Instance.GetPlayerManager();
            if (player == null) return;

            player.PlayerHit(BulletDamage);
        }
        else if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (IsHeadShot)
            {
                _animator.SetTrigger("BulletHeadShotDamage");
                _bulletHeadShotDamageText.text = BulletDamage.ToString();
            }
            else
            {
                _animator.SetTrigger("BulletDamage");
                _bulletDamageText.text = BulletDamage.ToString();
            }

            if (collider.gameObject.TryGetComponent(out Enemy enemy))
            {
                _classicBulletBloodSplatter.OnEnemyHit(transform);
                enemy.TakeDamage(BulletDamage, StunTimer, IsHeadShot);
            }
        }
    }

    private IEnumerator waitBeforeReturnBulletToPool()
    {
        yield return new WaitForSeconds(_bulletLifeTime);
        GameData.Instance.BulletPool.ReturnGameObject(gameObject);
    }

    private void HideBullet()
    {
        _currentVelocity = 0;
        _trailRenderer.time = 0.1f;
        _collider.enabled = false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.rotation * new Vector3(0, 0, 5f));
    }
}
