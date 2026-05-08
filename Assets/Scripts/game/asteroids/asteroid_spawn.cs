using System.Linq.Expressions;
using UnityEngine;

public class asteroid_spawn : MonoBehaviour
{
    public asteroid_prefab astroid_prefabs;
    public float max_spawn_distance;
    private float last_t = 0;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > last_t + 3.0f) {
            spawn_asteroid();
            last_t = Time.time;    
        }
    }

    void spawn_asteroid()
    {
        float random_distance = UnityEngine.Random.value * 2.0f - 1.0f;
        random_distance *= max_spawn_distance;

        float random_angle = UnityEngine.Random.value * 2.0f - 1.0f;

        int r = UnityEngine.Random.Range(0, astroid_prefabs.asteroids_l.Length);
        GameObject asteroid = Instantiate(astroid_prefabs.asteroids_l[r], transform.position + new Vector3(random_distance, 0.0f, 0.0f), transform.rotation);
        asteroid.transform.Rotate(new Vector3(0.0f, random_angle * 45, 0.0f));
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();

        rb.AddForce(asteroid.transform.forward * 400, ForceMode.Force);
        Destroy(asteroid, 15);

    }
}
