using System.Linq.Expressions;
using UnityEngine;

public class asteroid_spawn : MonoBehaviour
{
    public asteroid_prefab astroid_prefabs;
    public float max_spawn_distance;

    public void spawn_asteroid()
    {
        float random_distance = UnityEngine.Random.value * 2.0f - 1.0f;
        random_distance *= max_spawn_distance;

        float random_angle = UnityEngine.Random.value * 2.0f - 1.0f;

        int r = UnityEngine.Random.Range(0, astroid_prefabs.asteroids_l.Length);
        GameObject asteroid = Instantiate(astroid_prefabs.asteroids_l[r], transform.position + (transform.right * random_distance), transform.rotation);
        asteroid.transform.Rotate(new Vector3(0.0f, random_angle * 45, 0.0f));
        asteroid.transform.localScale *= 2.0f;
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();

        rb.AddForce(asteroid.transform.forward * 400, ForceMode.Force);
    }
}
