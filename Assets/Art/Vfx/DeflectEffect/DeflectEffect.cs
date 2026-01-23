using UnityEngine;

public class DeflectEffect : MonoBehaviour
{
    public ParticleSystem Particles;
    public float UpdatesPerSecond = 12f;
    public float Duration = 1f;

    private float elapsed = 0f;
    private float elapsedTotal = 0f;

    void Start()
    {
        Particles.randomSeed = (uint)Random.Range(uint.MinValue, uint.MaxValue);
    }

    void Update()
    {
        if (elapsedTotal >= Duration)
        {
            Destroy(gameObject);
            return;
        }

        float step = 1f / UpdatesPerSecond;

        elapsed += Time.deltaTime;
        elapsedTotal += Time.deltaTime;

        while (elapsed >= step)
        {
            Particles.Simulate(
                step,
                withChildren: true,
                restart: false,
                fixedTimeStep: false
                );

            elapsed -= step;
        }
    }

}
