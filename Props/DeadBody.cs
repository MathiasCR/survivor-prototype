using System.Collections;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _disappearTimer;

    private void OnEnable()
    {
        _animator.SetTrigger("Idle");
        StartCoroutine(WaitBeforeDisappear());
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator WaitBeforeDisappear()
    {
        yield return new WaitForSeconds(_disappearTimer);
        _animator.SetTrigger("Disappear");
    }

    public void DisappearEnded()
    {
        //GameData.Instance.DeadBodyPool.ReturnGameObject(gameObject);
    }
}
