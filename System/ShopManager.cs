using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerSpawn;
    [SerializeField] private List<ShopCrateBehaviour> shopCrates;
    [SerializeField] private CinemachineVirtualCamera _camera;

    public static ShopManager Instance { get; private set; }

    private PlayerManager _playerManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _playerManager = GameManager.Instance.GetPlayerManager();
        _playerManager.gameObject.transform.position = _playerSpawn.transform.position;

        foreach (ShopCrateBehaviour shopCrate in shopCrates)
        {
            shopCrate.OnPlayerGetMod += GameManager.Instance.OnPlayerGetMod;
            shopCrate.OnPlayerGetFilterGaz += GameManager.Instance.OnPlayerGetGazFilter;
        }

        _camera.LookAt = _playerManager.transform;
        _camera.Follow = _playerManager.transform;
        _camera.enabled = true;
    }

    public void OnShopLeave()
    {
        _camera.enabled = false;
    }
}
