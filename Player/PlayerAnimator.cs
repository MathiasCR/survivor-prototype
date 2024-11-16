using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private int _aimingLayer;
    private PlayerManager _playerManager;
    private WeaponManager _weaponManager;
    private float _currentLayerWeightVelocity = 0.0f;

    private void Start()
    {
        _playerManager = GameManager.Instance.GetPlayerManager();
        _playerManager.OnEquippedWeaponChange += OnEquippedWeaponChange;
    }

    private void Update()
    {
        if (_animator != null)
        {
            _aimingLayer = _animator.GetLayerIndex("Aiming");
            if (_playerManager.IsAiming && _playerManager.EquippedWeapon != null)
            {
                _animator.SetFloat("MoveAnimSpeed", _playerManager.MoveAnimationSpeed / 2);
            }
            else
            {
                _animator.SetFloat("MoveAnimSpeed", _playerManager.MoveAnimationSpeed);
            }

            if (_weaponManager != null)
            {
                float m_currentLayerWeight = _animator.GetLayerWeight(_aimingLayer);
                m_currentLayerWeight =
                    Mathf.SmoothDamp(m_currentLayerWeight, _playerManager.IsAiming ? 1 : 0, ref _currentLayerWeightVelocity, _weaponManager.AimingSpeed / 100);
                _animator.SetLayerWeight(_aimingLayer, m_currentLayerWeight);
            }
        }
    }

    private void OnEquippedWeaponChange()
    {
        if (_playerManager.EquippedWeapon.TryGetComponent(out WeaponManager weaponManager))
        {
            _weaponManager = weaponManager;
        }
    }

    public void WalkAnimation()
    {
        _animator.SetBool("IsWalking", true);
        _animator.SetBool("IsStrafing", false);
    }

    public void StrafAnimation()
    {
        _animator.SetBool("IsStrafing", true);
        _animator.SetBool("IsWalking", false);
    }

    public void IdleAnimation()
    {
        _animator.SetBool("IsStrafing", false);
        _animator.SetBool("IsWalking", false);
    }
}
