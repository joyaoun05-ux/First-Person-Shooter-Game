using UnityEngine;
using Cinemachine;

public class DamageImpulse : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
    }

    public void PlayImpulse()
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }
}