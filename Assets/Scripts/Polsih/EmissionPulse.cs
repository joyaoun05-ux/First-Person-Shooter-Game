using UnityEngine;

public class EmissionPulse : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 5f;

    private Material mat;

    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        mat = rend.material;
    }

    private void Update()
    {
        float emission = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        mat.SetColor("_EmissionColor", Color.yellow * emission);
    }
}