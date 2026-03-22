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
        float intensity = Mathf.Lerp(50,100, Mathf.PingPong(Time.time, 5));
        _light.color = result;
        _light.intensity = intensity;
    }
}
