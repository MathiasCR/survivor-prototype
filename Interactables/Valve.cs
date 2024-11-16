using UnityEngine;

public class Valve : Interactable
{
    [SerializeField] private DoorType _doorType;

    public override void Interact()
    {
        if (animator != null) animator.SetTrigger("Activate");

        switch (_doorType)
        {
            case DoorType.ToLobby:
                //Retourner au lobby
                GameManager.Instance.OnPlayerReturnToLobby();
                break;
            case DoorType.ToNextLevel:
                //Load le prochain level du biome déjà selectionné
                GameManager.Instance.OnPlayerGoesToNextLevel();
                break;
            case DoorType.ToBiomeSelection:
                //Afficher le GUI pour selectionner le biome du prochain run
                GameManager.Instance.OnPlayerChooseNextLevel();
                break;
        }
    }
}

public enum DoorType
{
    ToLobby,
    ToNextLevel,
    ToBiomeSelection,
}
