using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    
    [Header("Emission")]
    [SerializeField] private float baseEmissionRate;
    [SerializeField] private float growthRate = .25f;
    private float emissionRate;

    [Header("Speed")]
    private float baseSpeed = 1.2f;
    private float speed;

    void Start()
    {
        SetEmissionRate(baseEmissionRate);
        SetSpeed(baseSpeed);
    }

    public void ChangeBasedOnStreak(int streak, bool correctAnswer)
    {
        SetEmissionRate(streak * growthRate + baseEmissionRate);
        SetSpeed(streak * 0.3f + baseSpeed);
    }
    
    public void SetEmissionRate(float rate)
    {
        emissionRate = rate;
        var emission = particles.emission;
        emission.rateOverTime = emissionRate;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        var main = particles.main;
        main.simulationSpeed = speed;
    }
}