using UnityEngine;

public class ShieldPulse : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float intensity = 2f;

    private Material mat;

    private void Start()
    {
        mat = rend.material;
    }

    private void Update()
    {
        float emission = Mathf.Sin(Time.time * speed) * 0.5f + 0.5f;
        mat.SetColor("_EmissionColor", Color.cyan * emission * intensity);
    }
}