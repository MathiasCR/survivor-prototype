using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject _defaultCharacterPrefab;
    [SerializeField] private List<PlayableCharacter> _charactersProp;
    [SerializeField] private CinemachineVirtualCamera _camera;

    public static LobbyManager Instance { get; private set; }

    private PlayableCharacter _selectedCharacter;
    private PlayerManager _playerManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _playerManager = GameManager.Instance.GetPlayerManager();

        foreach (PlayableCharacter character in _charactersProp)
        {
            character.OnSelectedCharacterChange += OnSelectedCharacterChange;
        }

        SetSelectedCharacter(_playerManager.CharacterPrefab);
        _camera.Follow = _playerManager.transform;
        _camera.LookAt = _playerManager.transform;
        _camera.enabled = true;
    }

    public void OnLobbyLeave()
    {
        _camera.enabled = false;
    }

    public void SetSelectedCharacter(GameObject characterPrefab)
    {
        _selectedCharacter = _charactersProp.Find((prop) => prop.name == characterPrefab.name);
        _selectedCharacter.gameObject.SetActive(false);
    }

    private void OnSelectedCharacterChange(PlayableCharacter newSelectedCharacter)
    {
        _selectedCharacter.gameObject.SetActive(true);
        _selectedCharacter = newSelectedCharacter;
    }
}
