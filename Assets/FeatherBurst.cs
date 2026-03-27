using UnityEngine;

public class FeatherBurst : MonoBehaviour
{
    public ParticleSystem featherParticles;

    public void EmitFeathers(int amount)
    {
        var emitParams = new ParticleSystem.EmitParams();
        featherParticles.Emit(amount);
    }
}