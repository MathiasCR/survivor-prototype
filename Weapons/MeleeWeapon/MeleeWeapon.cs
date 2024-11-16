using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Melee Attack hit : " + collider.name);
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerManager player = GameManager.Instance.GetPlayerManager();
            if (player == null) return;

            Debug.Log("Hit player for : " + _enemy.EnemyDamage);
            player.PlayerHit(_enemy.EnemyDamage);
        }
    }
}
