using UnityEngine;

public class AlarmSiren : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _minDistanceToPlay;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        _audioSource.enabled = distance <= _minDistanceToPlay;
    }
}
