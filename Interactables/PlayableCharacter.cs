using System;
using UnityEngine;

public class PlayableCharacter : Interactable
{
    [SerializeField] private GameObject _characterPrefab;

    public event Action<PlayableCharacter> OnSelectedCharacterChange;

    public override void Interact()
    {
        PlayerManager _playerManager = GameManager.Instance.GetPlayerManager();
        _playerManager.SetPlayerCharacter(_characterPrefab);
        gameObject.SetActive(false);
        OnSelectedCharacterChange?.Invoke(this);
    }
}
