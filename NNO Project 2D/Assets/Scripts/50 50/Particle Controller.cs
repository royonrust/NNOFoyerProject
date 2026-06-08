using System.Collections;
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
        StopAllCoroutines();
        
        SetEmissionRate(streak * growthRate + baseEmissionRate);
        SetSpeed(streak * 0.3f + baseSpeed);

        if (correctAnswer) StartCoroutine(ParticleBurst(5f, 0.75f));
        else StartCoroutine(ParticleBurst(50000f, 0.3f));
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

    private IEnumerator ParticleBurst(float mult, float duration)
    {
        var main = particles.main;
        float originalSpeed = main.simulationSpeed;

        main.simulationSpeed = originalSpeed + mult;

        yield return null;
        yield return null;
        yield return null;

        float elapsed = 0f;
        float easeDuration = duration;

        while (elapsed < easeDuration)
        {
            elapsed += Time.deltaTime;
            main.simulationSpeed = Mathf.Lerp(originalSpeed + mult, originalSpeed, elapsed / easeDuration);
            yield return null;
        }

        main.simulationSpeed = originalSpeed;
    }
}