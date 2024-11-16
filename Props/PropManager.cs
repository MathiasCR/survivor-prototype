using UnityEngine;

public class PropManager : MonoBehaviour
{
    public static PropManager Singleton { get; private set; }

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    public void InstantiateDeadBody(Vector3 position, Quaternion rotation)
    {
        //GameObject deadBody = GameData.Instance.DeadBodyPool.GetGameObject(position, rotation);
        //deadBody.SetActive(true);
    }
}
