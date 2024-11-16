using UnityEngine;

public class TradeItem : Collectable
{
    [SerializeField] private MeshRenderer _tpRenderer;
    [SerializeField] private MeshRenderer _cigsRenderer;
    [SerializeField] private MeshRenderer _vodkaRenderer;
    [SerializeField] private MeshRenderer _pornRenderer;

    protected override void OnCollect()
    {
        GameManager.Instance.OnTradeItemCollected(_rarity);
    }

    protected override void SetRenderer()
    {
        switch (_rarity)
        {
            case Rarity.Common:
                _tpRenderer.enabled = true;
                break;
            case Rarity.Rare:
                _cigsRenderer.enabled = true;
                break;
            case Rarity.Epic:
                _vodkaRenderer.enabled = true;
                break;
            case Rarity.Legendary:
                _pornRenderer.enabled = true;
                break;
        }
    }
}
