using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button _startGameBtn;

    private void Awake()
    {
        _startGameBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.OnStartGame();
        });
    }
}
