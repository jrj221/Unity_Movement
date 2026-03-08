using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // NOTE: Script could be improved to serialize a selection of x or z to see which axis a platform should move in. 
    private Vector3 _pointOne;
    private Vector3 _pointTwo;
    [SerializeField] private float _movingRadius;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private bool _goOtherDirection;

    private void Awake()
    {
        _pointOne = new Vector3(transform.position.x - _movingRadius, transform.position.y, transform.position.z);
        _pointTwo = new Vector3(transform.position.x + _movingRadius, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * _moveSpeed + 0.5f, 1f);
        if (_goOtherDirection) t = 1f - t;
        transform.position = Vector3.Lerp(_pointOne,  _pointTwo, t);
    }
}
