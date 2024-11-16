using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private LineRenderer _laserRenderer;

    private void Update()
    {
        Vector3 laserTo = transform.localPosition + Vector3.forward * 3f;
        _laserRenderer.SetPosition(1, laserTo);
    }
}
