using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterParticle : MonoBehaviour
{
    [SerializeField] List<Material> _bloodSprayMaterials;
    [SerializeField] GameObject _bloodDecalParticleSystem;
    [SerializeField] ParticleSystem _bloodSplatterParticleSystem;

    private List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();
    private Transform _enemyTransform;

    public void OnEnemyHit(Transform enemy)
    {
        _enemyTransform = enemy;
        _bloodSplatterParticleSystem.Play();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(_bloodSplatterParticleSystem, other, _collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in _collisionEvents)
        {
            GenerateBloodDecal(collisionEvent);
        }
    }

    private void GenerateBloodDecal(ParticleCollisionEvent collisionEvent)
    {
        GameObject newBloodDecalParticleSystem = Instantiate(_bloodDecalParticleSystem, collisionEvent.intersection, Quaternion.identity);
        newBloodDecalParticleSystem.transform.LookAt(_enemyTransform);

        if (newBloodDecalParticleSystem.TryGetComponent(out ParticleSystemRenderer particleSystemRenderer))
        {
            particleSystemRenderer.material = _bloodSprayMaterials[Random.Range(0, _bloodSprayMaterials.Count)];
        }

        if (newBloodDecalParticleSystem.TryGetComponent(out ParticleSystem particleSystem))
        {
            ParticleSystem.MainModule main = particleSystem.main;
            float rotationY = newBloodDecalParticleSystem.transform.localRotation.eulerAngles.y + 35f;
            main.startRotationY = new ParticleSystem.MinMaxCurve(rotationY * Mathf.Deg2Rad, rotationY * Mathf.Deg2Rad);
            particleSystem.Emit(1);
        }
    }
}
