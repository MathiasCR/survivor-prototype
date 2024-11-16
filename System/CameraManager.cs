using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Animator _animator;

    PlayerManager _playerManager;

    private void Update()
    {
        /*if (_playerManager == null)
        {
            _playerManager = GameManager.Instance.GetPlayerManager();
        }
        else
        {
            transform.LookAt(_playerManager.transform.position);

            Vector3 newPos = _playerManager.transform.position + _offset;

            if (newPos.x > 0.1f || newPos.z > 0.1f)
            {
                transform.position = _playerManager.transform.position + _offset;
            }
        }*/
    }

    public void ActivateLobbyIntro()
    {
        _animator.SetTrigger("LobbyIntro");
    }

    public void LeaveLobby()
    {
        _animator.SetTrigger("LeaveLobby");
    }
}
