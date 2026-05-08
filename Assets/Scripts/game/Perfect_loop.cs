using UnityEngine;

public class Perfect_loop : MonoBehaviour
{
    public AudioSource source;
    
    // Los valores que me pasaste
    public int startLoopSample = 0;
    public int endLoopSample = 0;
    public int sample;

    void Update()
    {
        sample = source.timeSamples;
        // Si el audio llega al punto final del loop, saltamos al inicio del loop
        if (source.timeSamples >= endLoopSample)
        {
            source.timeSamples = startLoopSample;
        }
    }
}