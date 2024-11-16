using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _mousePos;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private PlayerManager _playerManager;

    private Vector2 _moveDir;
    private PlayerAnimator _playerAnimator;

    private void Start()
    {
        _playerAnimator = gameObject.GetComponentInChildren<PlayerAnimator>();
        _playerManager.OnCharacterChange += OnCharacterChange;

        GameManager.Instance.OnSceneChange += OnSceneChange;
    }

    private void Update()
    {
        if (_playerManager.IsAiming && _playerManager.EquippedWeapon != null)
        {
            _navMeshAgent.speed = _playerManager.AimingMoveSpeed;
        }
        else
        {
            _navMeshAgent.speed = _playerManager.MoveSpeed;
        }

        if (_playerManager.IsInMenu)
        {
            _navMeshAgent.isStopped = true;
            _playerAnimator.IdleAnimation();
        }
        else
        {
            _navMeshAgent.isStopped = false;
        }
    }

    private void FixedUpdate()
    {
        if (_playerManager.CanMove && _playerAnimator != null)
        {
            RotatePlayer();

            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        Vector3 direction = new Vector3(_moveDir.x, 0, _moveDir.y);
        Vector3 forwardCamera = Camera.main.transform.TransformDirection(direction);
        _navMeshAgent.SetDestination(transform.position + forwardCamera);

        if (forwardCamera.normalized.x == 0f && forwardCamera.normalized.z == 0f)
        {
            // No movement, stop both moving animations
            _playerManager.IsMoving = false;
            _playerAnimator.IdleAnimation();
        }
        else if (forwardCamera.normalized.z > 0.9f || forwardCamera.normalized.z < -0.9f)
        {
            _playerManager.IsMoving = true;
            _playerAnimator.WalkAnimation();
        }
        else
        {
            _playerManager.IsMoving = true;
            _playerAnimator.StrafAnimation();
        }
    }

    private void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Vector3 rotation = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z);
            transform.LookAt(rotation);
            _mousePos.position = rotation + new Vector3(0, 0.1f, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveDir = value.Get<Vector2>();
        _moveDir = moveDir;
    }

    private void OnCharacterChange()
    {
        _playerAnimator = _playerManager.CharacterPrefab.GetComponent<PlayerAnimator>();
    }

    private void OnSceneChange()
    {
        _navMeshAgent.destination = Vector3.zero;
    }
}
