using UnityEngine;
using UnityEngine.UI;

public class EnemyGUI : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    [SerializeField] private Transform _enemyHud;

    private void Update()
    {
        Vector3 rotation = Camera.main.transform.position;
        _enemyHud.transform.LookAt(rotation);
    }

    public void SetHealthBar(int health)
    {
        _healthBar.gameObject.SetActive(true);
        _healthBar.maxValue = health;
        _healthBar.value = health;
    }

    public void HideHealthBar()
    {
        _healthBar.gameObject.SetActive(false);
    }

    public void UpdateHealthBar(int health)
    {
        _healthBar.value = health;
    }
}
