using UnityEngine;

public class AlarmSiren : MonoBehaviour
{
    [SerializeField] private float _minDistanceToPlay;
    private GameObject _player;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _player = GameObject.Find("PlayerRoot"); // Won't let me assign in the prefab for some reason
    }
    
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        _audioSource.volume = distance <= _minDistanceToPlay ? 0.6f : 0;
    }
}
