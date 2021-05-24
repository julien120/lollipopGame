using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverParticleController : MonoBehaviour
{
    private float duration = 5.0f;
    [SerializeField] private ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [System.Obsolete]
    void Update()
    {

        float phi = Time.time / duration * 2 * Mathf.PI;
        float amplitude = Mathf.Cos(phi) * 0.5f + 0.5f;
        particle.startColor = Color.HSVToRGB(amplitude, 1, 1);
    }
}
