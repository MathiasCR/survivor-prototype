public class Extraction : Interactable
{
    public override void Interact()
    {
        //Open extraction menu
        GameManager.Instance.OnExtraction();
    }
}
