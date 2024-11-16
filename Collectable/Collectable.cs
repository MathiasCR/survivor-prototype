using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    [Header("To Initialize")]
    [SerializeField] protected float _magnetSpeed;
    [SerializeField] protected ParticleSystemRenderer _collectableParticleEffectRenderer;

    [Header("Rarity (Read Only)")]
    [SerializeField] protected Rarity _rarity;

    protected Vector3 _magnetDirection;
    protected PlayerManager _playerManager;
    protected bool _isInMagnetRadius = false;

    protected void Awake()
    {
        InvokeRepeating(nameof(GetPlayerCollider), 0.1f, 0.2f);

        _playerManager = GameManager.Instance.GetPlayerManager();
    }

    protected void Update()
    {
        if (_isInMagnetRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, _magnetDirection, Time.deltaTime * _magnetSpeed);
        }
    }

    public void SetRarity(Rarity rarity)
    {
        _rarity = rarity;

        _collectableParticleEffectRenderer.material = GameData.Instance.GetMaterialByRarity(_rarity);
        SetRenderer();
    }

    protected abstract void SetRenderer();

    public void MagnetToPlayer(Vector3 playerPos)
    {
        Vector3 direction = playerPos;
        direction.y = transform.position.y;
        _magnetDirection = direction;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (Utils.CompareLayerToLayerMask(other.gameObject.layer, GameLayers.Instance.PlayerLayer))
        {
            OnCollect();
            Destroy(gameObject);
        }
    }

    protected void GetPlayerCollider()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 1.2f, 0f), _playerManager.MagnetRadius, GameLayers.Instance.PlayerLayer);

        if (colliders.Length > 0)
        {
            _isInMagnetRadius = true;
            MagnetToPlayer(colliders[0].transform.position);
        }
        else
        {
            _isInMagnetRadius = false;
        }
    }

    protected abstract void OnCollect();
}

public enum LootType
{
    Mod,
    Filter,
    Artefact,
    ToiletPaper,
    Cigaret,
    Vodka,
    PornMagazine
}
