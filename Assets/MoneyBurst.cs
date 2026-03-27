using UnityEngine;

public class MoneyBurst : MonoBehaviour
{
    public ParticleSystem moneyParticles;

    public void EmitMoney(int amount)
    {
        var emitParams = new ParticleSystem.EmitParams();
        moneyParticles.Emit(amount);
    }
}