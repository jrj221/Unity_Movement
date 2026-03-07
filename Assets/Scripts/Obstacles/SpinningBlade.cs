using UnityEngine;

public class SpinningBlade : MonoBehaviour
{
    [SerializeField] private float _spinSpeed;

    private void FixedUpdate()
    {
       transform.Rotate(0, 0, Time.fixedDeltaTime * 10f * _spinSpeed);
    }
}