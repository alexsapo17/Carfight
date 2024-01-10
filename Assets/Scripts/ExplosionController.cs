using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public ParticleSystem explosionParticles;

    void Start()
    {
       explosionParticles.Simulate(0.02f, true, true);
        explosionParticles.Pause();
    }
}
