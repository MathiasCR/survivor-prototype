using UnityEngine;

public class WeaponSoundManager : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayFireSound(AudioClip fireClip)
    {
        // Play fire sound
        _audioSource.time = 0.015f;
        _audioSource.PlayOneShot(fireClip);
    }

    public void PlayReloadSound(AudioClip reloadClip)
    {
        // Play reload sound
    }
}
