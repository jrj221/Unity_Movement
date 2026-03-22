using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _laserDeath;

    private void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayLaserDeath()
    {
        _audioSource.PlayOneShot(_laserDeath);
    }
}
