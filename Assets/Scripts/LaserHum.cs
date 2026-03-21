using UnityEngine;

public class LaserHum : MonoBehaviour
{
    private GameObject _player;
    private AudioSource _audioSource;
    private float _maxDistance;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("PlayerRoot");
        _maxDistance = _audioSource.maxDistance;
    }
    
    
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance < _maxDistance && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        } else if (distance > _maxDistance && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
