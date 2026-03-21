using Unity.Mathematics;
using UnityEngine;

public class AlarmFlash : MonoBehaviour
{
    private Light _light;
    [SerializeField] private Color _color1;
    [SerializeField] private Color _color2;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }
    private void Update()
    {
        Color result =  Color.Lerp(_color1, _color2, Mathf.PingPong(Time.time, .5f));
        _light.color = result;
    }
}
