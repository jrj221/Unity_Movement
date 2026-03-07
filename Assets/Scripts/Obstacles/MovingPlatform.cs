using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 _pointOne;
    private Vector3 _pointTwo;
    [SerializeField] private float _movingRadius;
    [SerializeField] private float _moveSpeed;

    private void Awake()
    {
        _pointOne = new Vector3(transform.position.x - _movingRadius, transform.position.y, transform.position.z);
        _pointTwo = new Vector3(transform.position.x + _movingRadius, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * _moveSpeed + 0.5f, 1f);
        transform.position = Vector3.Lerp(_pointOne,  _pointTwo, t);
    }
}
