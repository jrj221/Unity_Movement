using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour
{
    [SerializeField] private Transform _platform;
    [SerializeField] private Transform _player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = _platform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = _player;
        }
    }
}
