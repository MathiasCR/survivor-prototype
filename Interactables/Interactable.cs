using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    [SerializeField] protected bool holdToUse;
    [SerializeField] protected float holdTimer;
    [SerializeField] protected bool isInteractable;
    [SerializeField] protected string interactionText;

    public bool IsInteractable
    {
        get => isInteractable;
    }

    public bool HoldToUse
    {
        get => holdToUse;
    }

    public float HoldTimer
    {
        get => holdTimer;
    }

    public string InteractionText
    {
        get => interactionText;
    }

    public abstract void Interact();
}
